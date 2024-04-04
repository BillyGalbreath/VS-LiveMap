import * as L from "leaflet";
import {LiveMap} from "../LiveMap";
import {Util} from "../util/Util";

export interface Marker {
    type: string,
    point: L.PointExpression,
    options: L.MarkerOptions;
}

export interface IconOptions extends L.MarkerOptions, L.IconOptions {
    // this lets us combine marker and icon options into one set
}

export class MarkersLayer extends L.LayerGroup {
    private readonly _types: {
        circle: (json: Marker) => L.Circle,
        ellipse: (json: Marker) => L.Ellipse,
        icon: (json: Marker) => L.Marker,
        polygon: (json: Marker) => L.Polygon,
        polyline: (json: Marker) => L.Polyline,
        //rectangle: (json: Type) => L.Rectangle
    } = {
        "circle": (json: Marker) => L.circle(Util.toLatLng(json.point), {
            ...json.options,
            radius: Util.pixelsToMeters((json.options as L.CircleOptions).radius!)
        }),
        "ellipse": (json: Marker) => L.ellipse(Util.toLatLng(json.point), json.options as L.EllipseOptions),
        "icon": (json: Marker) => L.marker(Util.toLatLng(json.point), {
            ...json.options,
            "icon": L.icon({...json.options as IconOptions})
        }),
        "polygon": (json: Marker) => L.polygon([Util.toLatLng(json.point)], json.options as L.PolylineOptions),
        "polyline": (json: Marker) => L.polyline([Util.toLatLng(json.point)], json.options as L.PolylineOptions),
        //"rectangle": (json: Type) => L.rectangle([window.toLatLng(json.point)], json.options as L.PolylineOptions)
    }

    private readonly _interval: number;

    constructor(livemap: LiveMap, markers: Marker[], interval?: number, options?: L.LayerOptions) {
        super([], options);
        this._interval = interval ?? 300;

        // todo - add if default show
        //if (true) {
        this.addTo(livemap);
        //}

        markers.forEach((marker: Marker): void => {
            try {
                const type = this._types[marker.type as keyof typeof this._types];
                if (type) {
                    type(marker)?.addTo(this);
                }
            } catch (e) {
                console.error(e);
            }
        });
    }

    public tick(count: number): void {
        if (count % this._interval == 0) {
            //
        }
    }
}
