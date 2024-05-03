import * as L from 'leaflet';
import {TileLayerControl} from './control/TileLayerControl';
import {LayersControl} from './control/LayersControl';
import {LinkControl} from './control/LinkControl';
import {CoordsControl} from './control/CoordsControl';
import {SidebarControl} from './control/SidebarControl';
import {ContextMenu} from './layer/menu/ContextMenu';
import {Notifications} from './layer/Notifications';
import {Settings} from './data/Settings';
import {Point} from './data/Point';
import './scss/styles';
import './svg'

export class LiveMap extends L.Map {
    declare _controlCorners: { [x: string]: HTMLDivElement; };
    declare _controlContainer?: HTMLElement;
    declare _container?: HTMLElement;

    private readonly _settings: Settings;

    private readonly _tileLayerControl: TileLayerControl;
    private readonly _layersControl: LayersControl;
    private readonly _linkControl: LinkControl;
    private readonly _coordsControl: CoordsControl;
    private readonly _sidebarControl: SidebarControl;

    private readonly _contextMenu: ContextMenu;
    private readonly _notifications: Notifications;

    private readonly _scale: number;

    constructor(settings: Settings) {
        // create the map div element
        L.DomUtil.create('div', 'loading', document.body).id = 'map';

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

        this._settings = settings;

        // set custom page title from lang
        document.title = settings.lang.title ?? 'Vintage Story LiveMap';

        // pre-calculate map's scale
        this._scale ??= (1 / Math.pow(2, settings.zoom.maxout));

        // set up the controllers
        this._tileLayerControl = new TileLayerControl(this);
        this._layersControl = new LayersControl(this);
        this._coordsControl = new CoordsControl(this);
        this._linkControl = new LinkControl(this);
        this._sidebarControl = new SidebarControl(this);

        // the fancy context menu and stuff
        this._contextMenu = new ContextMenu(this);
        this._notifications = new Notifications();

        // replace leaflet's attribution with our own
        this.attributionControl.setPrefix(settings.attribution);

        // stuff to do after the map fully loads
        this.on('load', (): void => this.onLoad());

        // center the map on url coordinates or spawn (0, 0); this initializes the map
        setTimeout((): void => {
            const url: URLSearchParams = new URLSearchParams(window.location.search);
            this.sidebarControl.renderersControl.rendererType = url.get('renderer');
            this.centerOn(
                Point.of(url.get('x') ?? 0, url.get('z') ?? 0),
                url.get('zoom') ?? this.settings.zoom.def
            );
        }, 0);
    }

    onLoad(): void {
        // get rid of the page logo and loading images
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

    private loop(count: number): void {
        try {
            if (document.visibilityState === 'visible') {
                this.tileLayerControl.tick(count);
                this.layersControl.tick(count);
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

    public updateSizeToWindow(): void {
        const style: CSSStyleDeclaration = this.getContainer().style;
        style.width = `${window.innerWidth}px`;
        style.height = `${window.innerHeight}px`;
        this.invalidateSize();
    }
}

window.onload = (): void => {
    window.fetchJson<Settings>('data/settings.json')
        .then((json: Settings): void => {
            window.livemap = new LiveMap(new Settings(json));
        })
        .catch((err: unknown): void => {
            console.error(`Error creating map\n`, err);
        });
};

// update map size when window size, scale, or orientation changes
"orientationchange resize".split(' ').forEach((event: string): void => {
    window.addEventListener(event, (): void => {
        window.livemap?.updateSizeToWindow();
    }, {passive: true});
});

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


const knownThemes: string[] = [];

for (let i: number = 0; i < document.styleSheets.length; i++) {
    const css: CSSStyleSheet = document.styleSheets[i];
    if (css.href?.endsWith('themes.css')) {
        const rules: CSSRuleList = css.cssRules;
        for (let j: number = 0; j < rules.length; j++) {
            const rule: CSSStyleRule = rules[j] as CSSStyleRule;
            const match: RegExpExecArray | null = /html\[theme="(.+)"]/.exec(rule.selectorText);
            if (match) {
                knownThemes.push(match[1]);
            }
        }
        break;
    }
}

window.matchMedia('(prefers-color-scheme: dark)')
    .addEventListener('change', (): void => setTheme());

const setTheme = (): void => {
    const prefersDark: boolean = knownThemes.length > 1 && window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme: string = localStorage.getItem('theme') ?? knownThemes[+prefersDark];
    document.querySelector('html')!.setAttribute('theme', theme);
    // todo
    //localStorage.setItem("theme", theme);
    //localStorage.removeItem("theme");
};

setTheme();
