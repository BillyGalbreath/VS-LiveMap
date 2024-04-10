import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Ellipse extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.ellipse(Util.tupleToLngLat(json.point), {
            ...json.options,
            radii: Ellipse.radius(json)
        }));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Ellipse)
            .setLatLng(Util.tupleToLngLat(data.point))
            .setRadius(Ellipse.radius(data));
    }

    private static radius(data: MarkerJson): L.PointTuple {
        return (data.options as L.EllipseOptions).radii ?? [10, 10];
    }
}
