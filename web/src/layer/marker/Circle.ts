import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Circle extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.circle(Location.of(json.point).toLatLng(), {
            ...json.options,
            radius: Circle.radius(json)
        }));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Circle)
            .setLatLng(Location.of(json.point).toLatLng())
            .setRadius(Circle.radius(json));
    }

    private static radius(json: MarkerJson): number {
        return Location.pixelsToMeters((json.options as L.CircleOptions).radius ?? 10);
    }
}
