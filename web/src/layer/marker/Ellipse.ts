import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";

export class Ellipse extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.ellipse(Location.of(json.point).toLatLng(), {
            ...json.options,
            radii: Ellipse.radius(json)
        }));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Ellipse)
            .setLatLng(Location.of(data.point).toLatLng())
            .setRadius(Ellipse.radius(data));
    }

    private static radius(data: MarkerJson): L.PointTuple {
        return (data.options as L.EllipseOptions).radii ?? [10, 10];
    }
}
