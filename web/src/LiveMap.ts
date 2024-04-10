import * as L from 'leaflet';
import {TileLayerControl} from './control/TileLayerControl';
import {LayersControl} from './control/LayersControl';
import {CoordsControl} from './control/CoordsControl';
import {LinkControl} from './control/LinkControl';
import {Settings} from './data/Settings';
import {ContextMenu} from "./util/ContextMenu";
import {Notifications} from "./util/Notifications";
import {Util} from './util/Util';
import './scss/styles';
import './svg/svgs'

window.onload = (): void => {
    // create map element
    const map: HTMLDivElement = L.DomUtil.create('div', 'loading', document.body);
    map.id = 'map';

    // fix map height for android devices
    // https://chanind.github.io/javascript/2019/09/28/avoid-100vh-on-mobile-web.html
    map.style.height = `${window.innerHeight}px`;

    Util.fetchJson('data/settings.json').then((json): void => {
        window.livemap = new LiveMap(json as Settings);
        window.livemap.init();
    });
};

export class LiveMap extends L.Map {
    private readonly _settings: Settings;

    private readonly _tileLayerControl: TileLayerControl;
    private readonly _layersControl: LayersControl;
    private readonly _linkControl: LinkControl;
    private readonly _coordsControl: CoordsControl;

    private readonly _contextMenu: ContextMenu;
    private readonly _notifications: Notifications;

    private readonly _scale: number;

    constructor(settings: Settings) {
        super('map', {
            // we need a flat and simple crs
            crs: L.Util.extend(L.CRS.Simple, {
                // we need to flip the y-axis correctly
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            // center map on spawn
            center: [settings.spawn.x, settings.spawn.y],
            // always allow attribution in case a layer needs it
            attributionControl: true,
            // canvas is more efficient than svg
            preferCanvas: true,
            // these get weird when changed
            zoomSnap: 1,
            zoomDelta: 1,
            wheelPxPerZoomLevel: 60
        });

        this.on('load', () => {
            this.getContainer().classList.remove('loading');
            document.querySelector('.logo svg')?.classList.remove('loading');
        })

        this._settings = settings;

        // set up the controllers
        this._tileLayerControl = new TileLayerControl(this);
        this._layersControl = new LayersControl(this);
        this._coordsControl = new CoordsControl(this);
        this._linkControl = new LinkControl(this);

        // the fancy context menu and stuff
        this._contextMenu = new ContextMenu(this);
        this._notifications = new Notifications();

        // replace leaflet's attribution with our own
        this.attributionControl.setPrefix(this._settings.attribution);

        // pre-calculate map's scale
        this._scale ??= (1 / Math.pow(2, this.settings.zoom.maxout));
    }

    // this has to be done _after_ the map has already been initialized
    init(): void {
        // move to the coords or spawn point at specified or default zoom level
        this.centerOn(
            this.getUrlParam('x', 0),
            this.getUrlParam('z', 0),
            this.getUrlParam('zoom', this.settings.zoom.def)
        );

        // start the tick loop
        this.loop(0);
    }

    get contextMenu(): ContextMenu {
        return this._contextMenu;
    }

    get coordsControl(): CoordsControl {
        return this._coordsControl;
    }

    get linkControl(): LinkControl {
        return this._linkControl
    }

    get layersControl(): LayersControl {
        return this._layersControl;
    }

    get notifications(): Notifications {
        return this._notifications;
    }

    get settings(): Settings {
        return this._settings;
    }

    get scale(): number {
        return this._scale;
    }

    private loop(count: number): void {
        try {
            if (document.visibilityState === 'visible') {
                this._tileLayerControl.tick(count);
                this._layersControl.tick(count);
            }
        } catch (e) {
            console.error(`Error processing tick (${count})\n`, e);
        }

        setTimeout(() => this.loop(++count), 1000);
    }

    public getOrCreatePane(name?: string): HTMLElement | undefined {
        return name ? this.getPane(name) ?? this.createPane(name) : undefined;
    }

    public centerOn(x: number, y: number, zoom?: number): void {
        if (zoom !== undefined) {
            this.setZoom(this.settings.zoom.maxout - zoom);
        }
        this.setView(Util.tupleToLatLng([
            x + this.settings.spawn.x,
            y + this.settings.spawn.y
        ]));
        this._linkControl.update();
    }

    public currentZoom(): number {
        return this.settings.zoom.maxout - this.getZoom();
    }

    public getUrlParam(query: string, def: number): number {
        return parseInt(new URLSearchParams(window.location.search).get(query) ?? `${def}`);
    }

    public getUrlFromView(): string {
        const center: L.Point = Util.toPoint(this.getCenter());
        const x: number = Math.floor(center.x) - this.settings.spawn.x;
        const y: number = Math.floor(center.y) - this.settings.spawn.y;
        return `?x=${x}&z=${y}&zoom=${this.currentZoom()}`;
    }
}

// https://stackoverflow.com/a/3955096
Array.prototype.remove = function <T>(obj: T, ax?: number): void {
    while ((ax = this.indexOf(obj)) !== -1) {
        this.splice(ax, 1);
    }
};
