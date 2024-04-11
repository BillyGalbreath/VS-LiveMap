import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";

export class Polyline extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.polyline(Location.toLatLngArray(json.points) as L.LatLng[], json.options));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Polyline).setLatLngs(Location.toLatLngArray(data.points) as L.LatLng[]);
    }
}
