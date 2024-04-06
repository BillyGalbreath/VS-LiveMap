import * as L from 'leaflet';
import {TileLayerControl} from './control/TileLayerControl';
import {MarkersControl} from './control/MarkersControl';
import {CoordsControl} from './control/CoordsControl';
import {LinkControl} from './control/LinkControl';
import {Settings} from './settings/Settings';
import {Util} from './util/Util';
import {ContextMenu} from './util/ContextMenu';
import './scss/styles';

window.onload = function (): void {
    // todo - add initial loading screen
    //

    // todo - add timeout error getting settings
    //

    Util.fetchJson('tiles/settings.json').then((json): void => {
        window.livemap = new LiveMap(json as Settings);
        window.livemap.init();
    });
};

export class LiveMap extends L.Map {
    private readonly _settings: Settings;

    private readonly _tileLayerControl: TileLayerControl;
    private readonly _markersControl: MarkersControl;
    private readonly _linkControl: LinkControl;
    private readonly _coordsControl: CoordsControl;

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

        this._settings = settings;

        // setup the controllers
        this._tileLayerControl = new TileLayerControl(this);
        this._markersControl = new MarkersControl(this);
        this._coordsControl = new CoordsControl(this);
        this._linkControl = new LinkControl(this);

        this._scale ??= (1 / Math.pow(2, this.settings.zoom.maxout));

        const contextmenu: ContextMenu = new ContextMenu(this);

        this.on('load', (): void => contextmenu.close());
        this.on('unload', (): void => contextmenu.close());
        this.on('resize', (): void => contextmenu.close());
        this.on('viewreset', (): void => contextmenu.close());

        this.on('move', (): void => contextmenu.close());
        this.on('movestart', (): void => contextmenu.close());
        this.on('moveend', (): void => contextmenu.close());

        this.on('zoom', (): void => contextmenu.close());
        this.on('zoomstart', (): void => contextmenu.close());
        this.on('zoomend', (): void => contextmenu.close());
        this.on('zoomlevelschange', (): void => contextmenu.close());

        this.on('popupopen', (): void => contextmenu.close());
        this.on('popupclose', (): void => contextmenu.close());
        this.on('tooltipopen', (): void => contextmenu.close());
        this.on('tooltipclose', (): void => contextmenu.close());

        this.on('click', (): void => contextmenu.close());
        this.on('dblclick', (): void => contextmenu.close());
        this.on('mousedown', (): void => contextmenu.close());
        this.on('mouseup', (): void => contextmenu.close());
        this.on('preclick', (): void => contextmenu.close());

        this.on('keydown', (e: L.LeafletKeyboardEvent): void => {
            if (e.originalEvent.key === 'Escape') {
                contextmenu.close();
            }
        });

        this.on('contextmenu', (e: L.LeafletMouseEvent): void => contextmenu.open(e));
    }

    init(): void {
        // pretty background
        this.getContainer().style.background = 'url(images/sky.png)';

        // replace leaflet's attribution with our own
        this.attributionControl.setPrefix(this._settings.attribution);

        // move to the coords or spawn point at specified or default zoom level
        this.centerOn(
            this.getUrlParam('x', 0),
            this.getUrlParam('y', 0),
            this.getUrlParam('zoom', this.settings.zoom.def)
        );

        // start the tick loop
        this.loop(0);
    }

    get coordsControl(): CoordsControl {
        return this._coordsControl;
    }

    get markersControl(): MarkersControl {
        return this._markersControl;
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
                this._markersControl.tick(count);
            }
        } catch (e) {
            console.error(`Error processing tick (${count})\n`, e);
        }

        setTimeout(() => this.loop(++count), 1000);
    }

    public centerOn(x: number, y: number, zoom: number): void {
        this.setZoom(this.settings.zoom.maxout - zoom);
        this.setView(Util.tupleToLatLng([
            x + this.settings.spawn.x,
            y + this.settings.spawn.y
        ]));
        this._linkControl.update();
    }

    public getUrlParam(query: string, def: number): number {
        return parseInt(new URLSearchParams(window.location.search).get(query) ?? `${def}`);
    }

    public getUrlFromView(): string {
        const center: L.Point = Util.toPoint(this.getCenter());
        const zoom: number = this.settings.zoom.maxout - this.getZoom();
        const x: number = Math.floor(center.x) - this.settings.spawn.x;
        const y: number = Math.floor(center.y) - this.settings.spawn.y;
        return `?x=${x}&y=${y}&zoom=${zoom}`;
    }
}

// https://stackoverflow.com/a/3955096
Array.prototype.remove = function <T>(obj: T, ax?: number): void {
    while ((ax = this.indexOf(obj)) !== -1) {
        this.splice(ax, 1);
    }
};
