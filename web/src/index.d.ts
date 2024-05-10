import * as L from 'leaflet';
import {LiveMap} from './LiveMap';
import {InteractiveLayerOptions} from "leaflet";

declare global {
    interface Window {
        livemap: LiveMap;

        fetchJson<T>(url: string): Promise<T>;

        createSVGIcon(icon: string): DocumentFragment
    }

    interface Array<T> {
        remove(obj: T): void;
    }
}

module 'leaflet' {
    export namespace Browser {
        const linux: boolean;
    }

    interface MarkerOptions extends InteractiveLayerOptions {
        iconUrl?: string;
        radii?: L.PointTuple;
        radius?: number;
        rotationAngle?: number;
    }

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
