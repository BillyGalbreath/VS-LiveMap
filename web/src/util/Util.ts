import * as L from "leaflet";
import {LngLat} from "./LngLat";

export class Util {
    public static isset(obj: string | number | null | undefined): boolean {
        return ![null, undefined, NaN, ''].includes(obj);
    }

    public static pixelsToMeters(num: number): number {
        return num * window.livemap.scale;
    }

    public static metersToPixels(num: number): number {
        return num / window.livemap.scale;
    }

    public static tupleToPoint(point: L.PointTuple | L.LatLngTuple): L.Point {
        return L.point(point[1], point[0]);
    }

    public static toPoint(latlng: L.LatLng): L.Point {
        if (latlng instanceof LngLat) {
            return L.point(Util.metersToPixels(latlng.lng), Util.metersToPixels(latlng.lat));
        } else {
            return L.point(Util.metersToPixels(latlng.lat), Util.metersToPixels(latlng.lng));
        }
    }

    public static tupleToLngLat(point: L.PointTuple | L.LatLngTuple): LngLat {
        return this.toLngLat([point[0], point[1]]);
    }

    public static toLngLat(point: L.PointTuple): LngLat {
        return new LngLat(Util.pixelsToMeters(point[0]), Util.pixelsToMeters(point[1]));
    }

    public static tupleToLatLng(point: L.PointTuple | L.LatLngTuple): L.LatLng {
        return this.toLatLng([point[0], point[1]]);
    }

    public static toLatLng(point: L.PointTuple): L.LatLng {
        return L.latLng(Util.pixelsToMeters(point[0]), Util.pixelsToMeters(point[1]));
    }

    public static toLngLatArray(tuples: MagicTuples): LngLat[] {
        for (let i: number = 0; i < tuples.length; i++) {
            let tuple: Voodoo = tuples[i];
            if (!Array.isArray(tuple)) {
                continue;
            }
            if (Array.isArray(tuple[0])) {
                tuples[i] = this.toLngLatArray(tuple);
            } else {
                tuples[i] = this.tupleToLngLat(tuple as L.LatLngTuple);
            }
        }
        return tuples as LngLat[];
    }

    public static async fetchJson(url: string) {
        const res: Response = await fetch(url, {
            headers: {
                "Content-Disposition": "inline"
            }
        });
        if (res.ok) {
            return await res.json();
        }
    }
}
