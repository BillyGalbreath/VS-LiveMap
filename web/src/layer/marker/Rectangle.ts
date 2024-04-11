import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";

export class Rectangle extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.rectangle(L.latLngBounds(Location.toLatLngArray(json.points) as L.LatLng[]), json.options));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Rectangle).setBounds(L.latLngBounds(Location.toLatLngArray(data.points) as L.LatLng[]));
    }
}
