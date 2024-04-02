import * as L from "leaflet";
import {LayerControl} from "./layer/LayerControl";
import {CoordsControl} from "./control/CoordsControl";
import {LinkControl} from "./control/LinkControl";
import {Point} from "./settings/Point";
import {Settings} from "./settings/Settings";

window.onload = function (): void {
    // todo - add initial loading screen
    //

    // todo - add timeout error getting settings
    //

    LiveMap.getJson("tiles/settings.json").then((json): void => {
        window.livemap = new LiveMap(json as Settings);
    });
};

export class LiveMap extends L.Map {
    private readonly _settings: Settings;
    private readonly _scale: number;

    private readonly _layerControl: LayerControl;

    private readonly _link: LinkControl;
    private readonly _coords: CoordsControl;

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
            // show attribution control box if we have an attribution
            attributionControl: LiveMap.isstr(settings.attribution),
            // canvas is more efficient than svg
            preferCanvas: true,
            zoomSnap: 1,
            zoomDelta: 1,
            wheelPxPerZoomLevel: 60
        });

        this._settings = settings;
        this._scale = (1 / Math.pow(2, this.settings.zoom.maxout));

        // replace leaflet's attribution with our own
        this.attributionControl?.setPrefix(settings.attribution);

        // move to the coords or spawn point at specified or default zoom level
        this.centerOn(
            parseInt(this.getUrlParam("x", 0)) + this.settings.spawn.x,
            parseInt(this.getUrlParam("z", 0)) + this.settings.spawn.x,
            parseInt(this.getUrlParam("y", this.settings.zoom.def))
        );

        // setup the layer controls (tile layers and layer overlays)
        this._layerControl = new LayerControl(this);

        // setup other control boxes
        this._coords = new CoordsControl(this);
        this._link = new LinkControl(this);

        // start the tick loop
        this.loop(0);
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
                this.tick(count);
            }
        } catch (e) {
            console.error(`Error processing tick (${count})`, e);
        }

        setTimeout(() => this.loop(count + 1), 1000);
    }

    private tick(count: number): void {
        // tick tiles
        if (count % this.settings.interval.tiles == 0) {
            this._layerControl.updateTileLayer();
        }

        // todo - tick player list
        if (count % this.settings.interval.players == 0) {
            //
        }

        // todo = tick marker layers
        if (count % this.settings.interval.markers == 0) {
            //
        }
    }

    public centerOn(x: number, z: number, zoom: number): void {
        this.setView(this.toLatLng(x, z), this.settings.zoom.maxout - zoom);
        this._link?.update();
    }

    public toLatLng(x: number, z: number): L.LatLng {
        return L.latLng(z * this.scale, x * this.scale);
    }

    public toPoint(latlng: L.LatLng): Point {
        return new Point(latlng.lng / this.scale, latlng.lat / this.scale);
    }

    public getUrlParam(query: string, def: any): any {
        return new URLSearchParams(window.location.search).get(query) ?? def;
    }

    public getUrlFromView(): string {
        const center: Point = this.toPoint(this.getCenter());
        const zoom: number = this.settings.zoom.maxout - this.getZoom();
        const x: number = Math.floor(center.x) - this.settings.spawn.x;
        const z: number = Math.floor(center.z) - this.settings.spawn.z;
        return `?x=${x}&z=${z}&y=${zoom}`;
    }

    public static async getJson(url: string): Promise<any> {
        let res: Response = await fetch(url, {
            headers: {
                "Content-Disposition": "inline"
            }
        });
        if (res.ok) {
            return await res.json();
        }
    }

    public static isset(obj: any): boolean {
        return ![null, undefined].includes(obj);
    }

    public static isstr(obj: any): boolean {
        return ![null, undefined, ''].includes(obj);
    }
}
