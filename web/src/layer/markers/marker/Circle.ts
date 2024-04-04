import * as L from "leaflet";
import {Util} from "../../../util/Util";

export class Circle extends L.CircleMarker {
    constructor(json: any) {
        super(Util.toLatLng(json.point), json.options);
    }
}
