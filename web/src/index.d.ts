import * as L from "leaflet";
import {LiveMap} from "./LiveMap";

declare global {
    interface Window {
        livemap: LiveMap;
    }

    // i don't even know :3 just go with it
    type MagicTuples = ArrayOfMagic
    interface Voodoo extends (MagicTuples | L.LatLng | number) {}
    interface ArrayOfMagic extends Array<Voodoo> {}
}

module "leaflet" {
    export function ellipse(latLng: L.LatLngExpression, options?: L.EllipseOptions): Ellipse;

    interface EllipseOptions extends L.PathOptions {
        radii?: L.PointTuple | undefined;
        tilt?: number;
    }
}
