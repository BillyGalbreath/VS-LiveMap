import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Rectangle extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.rectangle(L.latLngBounds(Util.toLngLatArray(json.points)), json.options));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Rectangle).setBounds(L.latLngBounds(Util.toLngLatArray(data.points)));
    }
}
