import * as L from 'leaflet';

export class Location {
    public static of(x: any, z?: number): Location {
        if (x === undefined || x === null) {
            return x;
        }
        if (x instanceof L.Point) {
            return new Location(x.x, x.y);
        }
        if (x instanceof L.LatLng) {
            return new Location(
                Location.metersToPixels(x.lng),
                Location.metersToPixels(x.lat)
            );
        }
        if (Array.isArray(x)) {
            return new Location(x[0], x[1]);
        }
        return new Location(x, z!);
    }

    public static toLatLngArray(arr: Location[] | L.LatLng[]): L.LatLng[] | Location | L.LatLng {
        for (let i: number = 0; i < arr.length; i++) {
            const entry: Location | L.LatLng = arr[i];
            if (!Array.isArray(entry)) {
                continue;
            }
            if (Array.isArray(entry[0])) {
                arr[i] = this.toLatLngArray(entry) as Location | L.LatLng;
            } else {
                arr[i] = Location.of(entry).toLatLng();
            }
        }
        return arr as L.LatLng[];
    }

    public static pixelsToMeters(num: number): number {
        return num * window.livemap.scale;
    }

    public static metersToPixels(num: number): number {
        return num / window.livemap.scale;
    }

    private _x: number;
    private _z: number;

    constructor(x: number, z: number) {
        this._x = x;
        this._z = z;
    }

    get x(): number {
        return this._x;
    }

    get z(): number {
        return this._z;
    }

    public add(n: number | Location): this {
        if (n instanceof Location) {
            this._x += n._x;
            this._z += n._z;
        } else {
            this._x += n;
            this._z += n;
        }
        return this;
    }

    public subtract(n: number | Location): this {
        if (n instanceof Location) {
            this._x -= n._x;
            this._z -= n._z;
        } else {
            this._x -= n;
            this._z -= n;
        }
        return this;
    }

    public multiply(n: number | Location): this {
        if (n instanceof Location) {
            this._x *= n._x;
            this._z *= n._z;
        } else {
            this._x *= n;
            this._z *= n;
        }
        return this;
    }

    public divide(n: number | Location): this {
        if (n instanceof Location) {
            this._x /= n._x;
            this._z /= n._z;
        } else {
            this._x /= n;
            this._z /= n;
        }
        return this;
    }

    public ceil(): this {
        this._x = Math.ceil(this._x);
        this._z = Math.ceil(this._z);
        return this;
    }

    public floor(): this {
        this._x = Math.floor(this._x);
        this._z = Math.floor(this._z);
        return this;
    }

    public round(): this {
        this._x = Math.round(this._x);
        this._z = Math.round(this._z);
        return this;
    }

    public toPoint(offset?: Location): L.Point {
        return L.point(
            this._x + (offset?._x ?? 0),
            this._z + (offset?._z ?? 0)
        );
    }

    public toLatLng(offset?: Location): L.LatLng {
        return L.latLng(
            Location.pixelsToMeters(this._z) + (offset?._z ?? 0),
            Location.pixelsToMeters(this._x) + (offset?._x ?? 0)
        );
    }

    public toString(format?: string): string {
        return (format ? format : '{x}, {z}')
            .replace('{x}', `${this._x}`)
            .replace('{z}', `${this._z}`);
    }
}
