import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {LiveMap} from "../../LiveMap";
import {Location} from "../../data/Location";

export class Icon extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.marker(Location.of(json.point).toLatLng(), {
            ...json.options,
            'icon': L.icon({...json.options as L.IconOptions})
        }));
    }

    public override update(data: MarkerJson): void {
        const icon: L.Marker = this._marker as L.Marker;
        icon.setLatLng(Location.of(data.point).toLatLng());
        Object.assign(icon.options, data.options);
        Object.assign(icon.options.icon!.options, data.options as L.IconOptions);
    }
}
