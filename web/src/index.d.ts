import * as L from "leaflet";
import {LiveMap} from "./LiveMap";

declare global {
    interface Window {
        livemap: LiveMap;
    }
}

module "leaflet" {
    export function ellipse(latLng: L.LatLngExpression, options?: L.EllipseOptions): Ellipse;

    interface EllipseOptions extends L.PathOptions {
        radii?: L.PointExpression | undefined;
        tilt?: number;
    }
}
