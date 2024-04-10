import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {Util} from "../../util/Util";
import {LiveMap} from "../../LiveMap";

export class Polygon extends Marker {
    constructor(livemap: LiveMap, json: MarkerJson) {
        super(livemap, json, L.polygon(Util.toLngLatArray(json.points), json.options));
    }

    public override update(data: MarkerJson): void {
        (this._marker as L.Polygon).setLatLngs(Util.toLngLatArray(data.points));
    }
}
