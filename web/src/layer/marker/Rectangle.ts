import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Rectangle extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.rectangle(L.latLngBounds(Location.toLatLngArray(json.points) as L.LatLng[]), json.options));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Rectangle).setBounds(L.latLngBounds(Location.toLatLngArray(json.points) as L.LatLng[]));
    }
}
