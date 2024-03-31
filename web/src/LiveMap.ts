import * as L from "leaflet";
import {Settings} from "./util/Settings";

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
    private readonly _scale: number;

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

        this.attributionControl?.setPrefix(settings.attribution);

        this._scale = (1 / Math.pow(2, settings.zoom.maxout));
    }

    get scale(): number {
        return this._scale;
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
