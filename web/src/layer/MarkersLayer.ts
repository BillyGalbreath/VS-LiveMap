import * as L from "leaflet";
import {LiveMap} from "../LiveMap";
import {Util} from "../util/Util";

interface Json {
    point: L.PointExpression,
    options: L.MarkerOptions;
}

export class MarkersLayer extends L.LayerGroup {
    private readonly _types: {
        circle: (json: Json) => L.Circle,
        ellipse: (json: Json) => L.Ellipse,
        icon: (json: Json) => L.Marker,
        polygon: (json: Json) => L.Polygon,
        polyline: (json: Json) => L.Polyline,
        //rectangle: (json: Type) => L.Rectangle
    } = {
        "circle": (json: Json) => L.circle(Util.toLatLng(json.point), {
            ...json.options,
            radius: Util.pixelsToMeters((json.options as L.CircleOptions).radius!)
        } as L.CircleOptions),
        "ellipse": (json: Json) => L.ellipse(Util.toLatLng(json.point), json.options as L.EllipseOptions),
        "icon": (json: Json) => L.marker(Util.toLatLng(json.point), {
            ...json.options,
            "icon": L.icon({...(json.options as any).icon as L.IconOptions})
        } as L.MarkerOptions),
        "polygon": (json: Json) => L.polygon([Util.toLatLng(json.point)], json.options as L.PolylineOptions),
        "polyline": (json: Json) => L.polyline([Util.toLatLng(json.point)], json.options as L.PolylineOptions),
        //"rectangle": (json: Type) => L.rectangle([window.toLatLng(json.point)], json.options as L.PolylineOptions)
    }

    private readonly _label: string;
    private readonly _interval: number;

    constructor(livemap: LiveMap, label: string, interval?: number, options?: L.LayerOptions) {
        super([], options);
        this._label = label;
        this._interval = interval ?? 60;

        // todo - add if default show
        if (true) {
            this.addTo(livemap);
        }

        // todo - fetch list of json files to process
        // each json file is its own layer full of markers
        Util.fetchJson("markers/spawn.json").then((json): void => {
            json.forEach((data: any): void => {
                try {
                    let type: any = this._types[data.type as keyof typeof this._types];
                    if (type) {
                        type(data)?.addTo(this);
                    }
                } catch (e) {
                    console.error("ERRoR", e);
                }
            });
        });
    }

    get label(): string {
        return this._label;
    }

    public tick(count: number): void {
        if (count % this._interval == 0) {
            //
        }
    }
}
