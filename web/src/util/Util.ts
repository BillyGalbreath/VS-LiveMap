import * as L from "leaflet";

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

    public static pointToPoint(point: L.PointExpression): L.Point {
        if (Array.isArray(point)) {
            return L.point(point[0], point[1]);
        }
        return point;
    }

    public static toLatLng(point: L.PointExpression): L.LatLng {
        point = Util.pointToPoint(point).add(window.livemap.settings.spawn);
        return L.latLng(point.y * window.livemap.scale, point.x * window.livemap.scale);
    }

    public static toPoint(latlng: L.LatLng): L.Point {
        return L.point(latlng.lng / window.livemap.scale, latlng.lat / window.livemap.scale);
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
