import * as L from "leaflet";
import {CoordsControl} from "./control/CoordsControl";
import {LinkControl} from "./control/LinkControl";
import {Point} from "./util/Point";
import {Settings} from "./util/Settings";
import {Zoom} from "./util/Zoom";

window.onload = function (): void {
    // todo - add initial loading screen
    //

    // todo - add timeout error getting settings
    //

    LiveMap.getJson("tiles/settings.json").then((json): void => {
        let settings: Settings = json as Settings;
        window.livemap = new LiveMap(settings);
    });
};

export class LiveMap extends L.Map {
    private readonly _settings: Settings;
    private readonly _scale: number;

    private readonly _link: LinkControl;
    private readonly _coords: CoordsControl;

    constructor(settings: Settings) {
        super('map', {
            crs: L.Util.extend(L.CRS.Simple, {
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            center: [settings.size.x / 2, settings.size.z / 2],
            attributionControl: LiveMap.isstr(settings.attribution),
            preferCanvas: true,
            zoomSnap: 1 / 4,
            zoomDelta: 1 / 4,
            wheelPxPerZoomLevel: 60 * 4
        });
        this.on('overlayadd', (e: L.LayersControlEvent): void => {
            //this.layerControls.showLayer(e.layer);
        });
        this.on('overlayremove', (e: L.LayersControlEvent): void => {
            //this.layerControls.hideLayer(e.layer);
        });

        this._settings = settings;
        this._scale = (1 / Math.pow(2, this.zoom.maxout));

        this.attributionControl?.setPrefix(settings.attribution);

        this.centerOn(
            parseInt(this.getUrlParam("x", 0)),
            parseInt(this.getUrlParam("z", 0)),
            parseInt(this.getUrlParam("y", this.zoom.def))
        );

        //this.layerControls.setupLayers();

        this._coords = new CoordsControl(this);
        this._link = new LinkControl(this);
    }

    get settings(): Settings {
        return this._settings;
    }

    get scale(): number {
        return this._scale;
    }

    get size(): Point {
        return this.settings.size;
    }

    get spawn(): Point {
        return this.settings.spawn;
    }

    get zoom(): Zoom {
        return this.settings.zoom;
    }

    public centerOn(x: number, z: number, zoom: number): void {
        this.setView(this.toLatLng(x + this.settings.spawn.x, z + this.settings.spawn.z), this.settings.zoom.maxout - zoom);
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
        const zoom: number = this.zoom.maxout - this.getZoom();
        const x: number = Math.floor(center.x) - this.spawn.x;
        const z: number = Math.floor(center.z) - this.spawn.z;
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
