import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Polyline extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.polyline(Location.toLatLngArray(json.points) as L.LatLng[], json.options));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Polyline).setLatLngs(Location.toLatLngArray(json.points) as L.LatLng[]);
    }
}
