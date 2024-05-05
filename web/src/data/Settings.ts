import {Point} from './Point';
import {Web} from './Web';
import {Zoom} from './Zoom';
import {Renderer} from './Renderer';
import {UI} from './UI';
import {Lang} from './Lang';

export class Settings {
    private readonly _attribution: string;
    private readonly _friendlyUrls: boolean;
    private readonly _interval: number;
    private readonly _size: Point;
    private readonly _spawn: Point;
    private readonly _web: Web;
    private readonly _zoom: Zoom;
    private readonly _renderers: Renderer[];
    private readonly _ui: UI;
    private readonly _lang: Lang;
    private readonly _markers: string[];

    constructor(json: Settings) {
        this._attribution = json.attribution ?? '';
        this._friendlyUrls = json.friendlyUrls ?? true;
        this._interval = json.interval ?? 30;
        this._size = json.size ? Point.of(json.size) : Point.of(1024000, 1024000);
        this._spawn = json.spawn ? Point.of(json.spawn) : this.size.divide(2);
        this._web = json.web ? new Web(json.web) : new Web();
        this._zoom = json.zoom ? new Zoom(json.zoom) : new Zoom();
        this._renderers = json.renderers ?? [];
        this._ui = json.ui ? new UI(json.ui) : new UI();
        this._lang = json.lang ? new Lang(json.lang) : new Lang();
        this._markers = json.markers ?? [];
    }

    get attribution(): string {
        return this._attribution;
    }

    get friendlyUrls(): boolean {
        return this._friendlyUrls;
    }

    get interval(): number {
        return this._interval;
    }

    get size(): Point {
        return this._size;
    }

    get spawn(): Point {
        return this._spawn;
    }

    get web(): Web {
        return this._web;
    }

    get zoom(): Zoom {
        return this._zoom;
    }

    get renderers(): Renderer[] {
        return this._renderers;
    }

    get ui(): UI {
        return this._ui;
    }

    get lang(): Lang {
        return this._lang;
    }

    get markers(): string[] {
        return this._markers;
    }
}
