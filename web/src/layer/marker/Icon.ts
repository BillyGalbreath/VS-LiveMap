import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Icon extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.marker(Util.tupleToLngLat(json.point), {
            ...json.options,
            'icon': L.icon({...json.options as L.IconOptions})
        }));
    }

    public override update(data: MarkerJson): void {
        const icon: L.Marker = this._marker as L.Marker;
        icon.setLatLng(Util.tupleToLngLat(data.point));
        Object.assign(icon.options, data.options);
        Object.assign(icon.options.icon!.options, data.options as L.IconOptions);
    }
}
