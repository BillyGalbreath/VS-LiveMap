import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Circle extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.circle(Util.tupleToLngLat(json.point), {
            ...json.options,
            radius: Circle.radius(json)
        }));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Circle)
            .setLatLng(Util.tupleToLngLat(data.point))
            .setRadius(Circle.radius(data));
    }

    private static radius(data: MarkerJson): number {
        return Util.pixelsToMeters((data.options as L.CircleOptions).radius ?? 10);
    }
}
