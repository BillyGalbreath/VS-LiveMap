import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Polyline extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.polyline(Util.toLngLatArray(json.points), json.options));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Polyline).setLatLngs(Util.toLngLatArray(data.points));
    }
}
