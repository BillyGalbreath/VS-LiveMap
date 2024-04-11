import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";

export class Circle extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.circle(Location.of(json.point).toLatLng(), {
            ...json.options,
            radius: Circle.radius(json)
        }));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Circle)
            .setLatLng(Location.of(data.point).toLatLng())
            .setRadius(Circle.radius(data));
    }

    private static radius(data: MarkerJson): number {
        return Location.pixelsToMeters((data.options as L.CircleOptions).radius ?? 10);
    }
}
