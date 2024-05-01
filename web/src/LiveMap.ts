import * as L from 'leaflet';
import {TileLayerControl} from './control/TileLayerControl';
import {LayersControl} from './control/LayersControl';
import {CoordsControl} from './control/CoordsControl';
import {LinkControl} from './control/LinkControl';
import {Point} from "./data/Point";
import {Settings} from './data/Settings';
import {ContextMenu} from "./layer/menu/ContextMenu";
import {Notifications} from "./layer/Notifications";
import './scss/styles';
import './svg'
import {SidebarControl} from "./control/SidebarControl";
import {PlayersControl} from "./control/PlayersControl";

//const prefersDarkScheme: MediaQueryList = window.matchMedia("(prefers-color-scheme: dark)");
const theme: string | null = localStorage.getItem("theme") ?? "glass";
//console.log(theme);
document.querySelector("html")!.setAttribute("theme", theme);
//localStorage.setItem("theme", theme);
//localStorage.removeItem("theme");

// update map size when window size, scale, or orientation changes
"orientationchange resize".split(' ').forEach((event: string): void => {
    window.addEventListener(event, (): void => {
        window.livemap?.updateSizeToWindow();
    }, {passive: true});
});

window.onload = (): void => {
    window.fetchJson<Settings>('data/settings.json')
        .then((json: Settings): void => {
            // create the map div element
            L.DomUtil.create('div', 'loading', document.body).id = 'map';

            // create the map itself
            window.livemap = new LiveMap(new Settings(json));

            // get url params
            const url: URLSearchParams = new URLSearchParams(window.location.search);

            // select the renderer
            window.livemap.rendererType = url.get('renderer') ?? 'basic';

            // center the map on url coordinates or spawn (0, 0); this initializes the map
            window.livemap.centerOn(
                Point.of(url.get('x') ?? 0, url.get('z') ?? 0),
                url.get('zoom') ?? window.livemap.settings.zoom.def
            );
        })
        .catch((err: unknown): void => {
            console.error(`Error creating map\n`, err);
        });
};

window.fetchJson = async <T>(url: string): Promise<T> => {
    const res: Response = await fetch(url, {
        headers: {
            "Content-Disposition": "inline"
        }
    });
    if (res.ok) {
        return await res.json();
    }
    throw (res.statusText);
}

window.createSVGIcon = (icon: string): DocumentFragment => {
    const template: HTMLTemplateElement = L.DomUtil.create('template');
    template.innerHTML = `<svg><use href='#svg-${icon}'></use></svg>`;
    return template.content;
}

// https://stackoverflow.com/a/3955096
Array.prototype.remove = function <T>(obj: T, ax?: number): void {
    while ((ax = this.indexOf(obj)) !== -1) {
        this.splice(ax, 1);
    }
};

export class LiveMap extends L.Map {
    declare _controlCorners: { [x: string]: HTMLDivElement; };
    declare _controlContainer?: HTMLElement;
    declare _container?: HTMLElement;

    private readonly _settings: Settings;

    private readonly _tileLayerControl: TileLayerControl;
    private readonly _layersControl: LayersControl;
    private readonly _linkControl: LinkControl;
    private readonly _coordsControl: CoordsControl;
    private readonly _playersControl: PlayersControl;
    private readonly _sidebarControl: SidebarControl;

    private readonly _contextMenu: ContextMenu;
    private readonly _notifications: Notifications;

    private readonly _scale: number;

    private _rendererType: string = "basic";

    constructor(settings: Settings) {
        super('map', {
            // we need a flat and simple crs
            crs: L.Util.extend(L.CRS.Simple, {
                // we need to flip the y-axis correctly
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            // center map on spawn
            center: [settings.spawn.x, settings.spawn.z],
            // always allow attribution in case a layer needs it
            attributionControl: true,
            // canvas is more efficient than svg
            preferCanvas: true,
            // these get weird when changed
            zoomSnap: 1,
            zoomDelta: 1,

            // chrome based browsers on linux zoom twice as fast, so we have to double the ratio
            // effectively undoes the fix for Leaflet/Leaflet#4538 and Leaflet/Leaflet#7403
            // https://github.com/Leaflet/Leaflet/commit/96977a19358374b0166cff049862fa1f0fed5948
            //
            // todo remove this logic when this bug gets fixed: https://issues.chromium.org/issues/40887377
            // it seems intentional, so it might not get fixed https://issues.chromium.org/issues/40804672
            wheelPxPerZoomLevel: L.Browser.linux && L.Browser.chrome ? 120 : 60
        });

        this.on('load', (): void => this.onLoad());

        this._settings = settings;

        document.title = settings.lang.title ?? 'Vintage Story LiveMap';

        // set up the controllers
        this._tileLayerControl = new TileLayerControl(this);
        this._layersControl = new LayersControl(this);
        this._coordsControl = new CoordsControl(this);
        this._linkControl = new LinkControl(this);
        this._playersControl = new PlayersControl(this);
        this._sidebarControl = new SidebarControl(this);

        // the fancy context menu and stuff
        this._contextMenu = new ContextMenu(this);
        this._notifications = new Notifications();

        // replace leaflet's attribution with our own
        this.attributionControl.setPrefix(this._settings.attribution);

        // pre-calculate map's scale
        this._scale ??= (1 / Math.pow(2, this.settings.zoom.maxout));
    }

    onLoad(): void {
        const container: HTMLElement = this.getContainer();
        container.classList.remove('loading');
        container.addEventListener('transitionend', (e: TransitionEvent): void => {
            if (e.target === container) {
                document.querySelector('.logo')?.remove();
            }
        }, {passive: true});

        // fix map size on load - fixes android browser url bar pushing page off-screen
        // https://chanind.github.io/javascript/2019/09/28/avoid-100vh-on-mobile-web.html
        this.updateSizeToWindow();

        // replace layers.png with an svg
        const layers: HTMLElement = document.querySelector('.leaflet-control-layers-toggle')!;
        layers.style.backgroundImage = 'none';
        layers.appendChild(window.createSVGIcon('layers'));
        const svg: SVGElement = layers.firstChild as SVGElement;
        svg.style.width = '24px';
        svg.style.height = '24px';
        svg.style.margin = '3px';

        // fix svg size issues in weird browsers like safari
        document.querySelectorAll('svg').forEach((svg: Element): void => {
            svg.setAttribute('preserveAspectRatio', 'none');
        });

        // start the tick loop
        this.loop(0);
    }

    // https://stackoverflow.com/a/60391674/3530727
    _initControlPos(): void {
        const container: HTMLDivElement = this._controlContainer = L.DomUtil.create('div', 'leaflet-control-container', this._container);
        const corners: { [x: string]: HTMLDivElement; } = this._controlCorners = {};

        function createRow(vSide: string): void {
            const div: HTMLDivElement = L.DomUtil.create('div', `leaflet-control-container-${vSide}`, container);
            createCell(vSide, 'left', div);
            createCell(vSide, 'center', div);
            createCell(vSide, 'right', div);
        }

        function createCell(vSide: string, hSide: string, container: HTMLDivElement): void {
            corners[`${vSide}${hSide}`] = L.DomUtil.create('div', `leaflet-${vSide} leaflet-${hSide}`, container);
        }

        createRow('top');
        createRow('middle');
        createRow('bottom');
    }

    get settings(): Settings {
        return this._settings;
    }

    get tileLayerControl(): TileLayerControl {
        return this._tileLayerControl;
    }

    get layersControl(): LayersControl {
        return this._layersControl;
    }

    get coordsControl(): CoordsControl {
        return this._coordsControl;
    }

    get linkControl(): LinkControl {
        return this._linkControl
    }

    get playersControl(): PlayersControl {
        return this._playersControl;
    }

    get sidebarControl(): SidebarControl {
        return this._sidebarControl;
    }

    get contextMenu(): ContextMenu {
        return this._contextMenu;
    }

    get notifications(): Notifications {
        return this._notifications;
    }

    get scale(): number {
        return this._scale;
    }

    get rendererType(): string {
        return this._rendererType;
    }

    set rendererType(renderer: string) {
        this._rendererType = renderer;
    }

    private loop(count: number): void {
        try {
            if (document.visibilityState === 'visible') {
                this.tileLayerControl.tick(count);
                this.layersControl.tick(count);
                this.playersControl.tick();
                this.sidebarControl.tick();
            }
        } catch (e) {
            console.error(`Error processing tick (${count})\n`, e);
        }

        setTimeout(() => this.loop(++count), 1000);
    }

    public createPaneIfNotExist(pane?: string): void {
        if (pane && this.getPane(pane) === undefined) {
            this.createPane(pane);
        }
    }

    public centerOn(point: Point, zoom?: number | string): void {
        if (zoom !== undefined) {
            this.setZoom(this.settings.zoom.maxout - +zoom);
        }
        this.setView(point.add(this.settings.spawn).toLatLng());
        this._linkControl.update();
    }

    public currentZoom(): number {
        return this.settings.zoom.maxout - this.getZoom();
    }

    public getUrlFromView(): string {
        const point: Point = Point.of(this.getCenter())
            .floor()
            .subtract(this.settings.spawn);
        return `?renderer=${this.rendererType}&x=${point.x}&z=${point.z}&zoom=${this.currentZoom()}`;
    }

    public updateSizeToWindow(): void {
        const style: CSSStyleDeclaration = this.getContainer().style;
        style.width = `${window.innerWidth}px`;
        style.height = `${window.innerHeight}px`;
        this.invalidateSize();
    }
}
