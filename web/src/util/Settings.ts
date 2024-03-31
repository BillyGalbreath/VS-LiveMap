import {Point} from "./Point";
import {Zoom} from "./Zoom";

export class Settings {
    private readonly _attribution: string;
    private readonly _size: Point;
    private readonly _spawn: Point;
    private readonly _zoom: Zoom;

    constructor(size?: Point, spawn?: Point, zoom?: Zoom, attribution?: string) {
        this._attribution = attribution ?? '';
        this._size = size ?? new Point(1024000, 1024000);
        this._spawn = spawn ?? this._size.div(2);
        this._zoom = zoom ?? new Zoom();
    }

    get attribution(): string {
        return this._attribution;
    }

    get size(): Point {
        return this._size;
    }

    get spawn(): Point {
        return this._spawn;
    }

    get zoom(): Zoom {
        return this._zoom;
    }
}
