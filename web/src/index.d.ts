import * as L from 'leaflet';
import {LiveMap} from './LiveMap';

declare global {
    interface Window {
        livemap: LiveMap;
    }

    interface Array<T> {
        remove(obj: T): void;
    }
}

module 'leaflet' {
    export function ellipse(latLng: L.LatLngExpression, options?: L.EllipseOptions): L.Ellipse;

    interface Ellipse extends L.Path {
        setRadius(radii: L.PointTuple): this;

        getRadius(): L.Point;

        setTilt(tilt: number): this;

        getBounds(): L.LatLngBounds;

        getLatLng(): L.LatLng;

        setLatLng(latLng: L.LatLngExpression): this;
    }

    interface EllipseOptions extends L.PathOptions {
        radii?: L.PointTuple | undefined;
        tilt?: number;
    }
}
