import * as L from "leaflet";
import {Util} from "../../../util/Util";

export class Icon extends L.Marker {
    constructor(json: any) {
        super(Util.toLatLng(json.point), json.options as L.MarkerOptions);
    }
}
