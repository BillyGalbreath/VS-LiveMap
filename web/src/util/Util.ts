import * as L from "leaflet";

export class Util {
    public static isset(obj: any): boolean {
        return ![null, undefined, NaN, ''].includes(obj);
    }

    public static pixelsToMeters(num: number): number {
        return num * window.livemap.scale;
    }

    public static metersToPixels(num: number): number {
        return num / window.livemap.scale;
    }

    public static pointToPoint(point: L.PointExpression): L.Point {
        let x, y;
        if (point instanceof L.Point) {
            x = point.x;
            y = point.y;
        }
        if (Array.isArray(point)) {
            x = (point as any)[0];
            y = (point as any)[1];
        }
        if (point === undefined || point === null) {
            return point;
        }
        if (typeof point === 'object' && 'x' in point && 'y' in point) {
            x = point.x;
            y = point.y;
        }
        return L.point(x, y);
    }

    public static toLatLng(point: L.PointExpression): L.LatLng {
        point = Util.pointToPoint(point);
        return L.latLng(point.y * window.livemap.scale, point.x * window.livemap.scale);
    }

    public static toPoint(latlng: L.LatLng): L.Point {
        return L.point(latlng.lng / window.livemap.scale, latlng.lat / window.livemap.scale);
    }

    public static async fetchJson(url: string): Promise<any> {
        let res: Response = await fetch(url, {
            headers: {
                "Content-Disposition": "inline"
            }
        });
        if (res.ok) {
            return await res.json();
        }
    }
}
