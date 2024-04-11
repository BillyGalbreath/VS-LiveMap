import {Location} from "./Location";
import {Zoom} from './Zoom';

export class Settings {
    private readonly _attribution: string;
    private readonly _interval: number;
    private readonly _size: Location;
    private readonly _spawn: Location;
    private readonly _zoom: Zoom;
    private readonly _markers: string[];

    constructor(json: Settings) {
        this._attribution = json.attribution ?? '';
        this._interval = json.interval ?? 30;
        this._size = json.size ? Location.of(json.size) : Location.of(1024000, 1024000);
        this._spawn = json.spawn ? Location.of(json.spawn) : this.size.divide(2);
        this._zoom = json.zoom ? new Zoom(json.zoom) : new Zoom();
        this._markers = json.markers ?? [];
    }

    get attribution(): string {
        return this._attribution;
    }

    get interval(): number {
        return this._interval;
    }

    get size(): Location {
        return this._size;
    }

    get spawn(): Location {
        return this._spawn;
    }

    get zoom(): Zoom {
        return this._zoom;
    }

    get markers(): string[] {
        return this._markers;
    }
}
