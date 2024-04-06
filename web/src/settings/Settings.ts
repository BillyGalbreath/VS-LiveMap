import * as L from 'leaflet';
import {Zoom} from './Zoom';

export class Settings {
    private readonly _attribution: string;
    private readonly _interval: number;
    private readonly _size: L.Point;
    private readonly _spawn: L.Point;
    private readonly _zoom: Zoom;

    constructor(size?: L.Point, spawn?: L.Point, zoom?: Zoom, interval?: number, attribution?: string) {
        this._attribution = attribution ?? '';
        this._interval = interval ?? 30;
        this._size = size ?? L.point(1024000, 1024000);
        this._spawn = spawn ?? this._size.divideBy(2);
        this._zoom = zoom ?? new Zoom();
    }

    get attribution(): string {
        return this._attribution;
    }

    get interval(): number {
        return this._interval;
    }

    get size(): L.Point {
        return this._size;
    }

    get spawn(): L.Point {
        return this._spawn;
    }

    get zoom(): Zoom {
        return this._zoom;
    }
}
