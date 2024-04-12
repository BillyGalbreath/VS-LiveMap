import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Ellipse extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.ellipse(Location.of(json.point).toLatLng(), {
            ...json.options,
            radii: Ellipse.radius(json)
        }));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Ellipse)
            .setLatLng(Location.of(json.point).toLatLng())
            .setRadius(Ellipse.radius(json));
    }

    private static radius(json: MarkerJson): L.PointTuple {
        return (json.options as L.EllipseOptions).radii ?? [10, 10];
    }
}
