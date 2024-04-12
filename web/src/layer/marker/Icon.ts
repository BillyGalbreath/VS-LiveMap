import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Icon extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        console.log(json)
        super(layer, json, L.marker(Location.of(json.point).toLatLng(), {
            ...json.options,
            'icon': L.icon({...json.options as L.IconOptions})
        }));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        const icon: L.Marker = this._marker as L.Marker;
        icon.setLatLng(Location.of(json.point).toLatLng());
        Object.assign(icon.options, json.options);
        Object.assign(icon.options.icon!.options, json.options as L.IconOptions);
    }
}
