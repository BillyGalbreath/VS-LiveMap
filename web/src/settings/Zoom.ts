export class Zoom {
    private readonly _def: number;
    private readonly _maxin: number;
    private readonly _maxout: number;
    private readonly _snap: number;
    private readonly _delta: number;
    private readonly _wheel: number;

    constructor(def?: number, min?: number, max?: number, snap?: number, delta?: number, wheel?: number) {
        this._def = def ?? 0;
        this._maxin = min ?? 2;
        this._maxout = max ?? 3;
        this._snap = snap ?? 1 / 4;
        this._delta = delta ?? 1 / 4;
        this._wheel = wheel ?? 60 * 4;
    }

    get def(): number {
        return this._def;
    }

    get maxin(): number {
        return this._maxin;
    }

    get maxout(): number {
        return this._maxout;
    }

    get snap(): number {
        return this._snap;
    }

    get delta(): number {
        return this._delta;
    }

    get wheel(): number {
        return this._wheel;
    }
}
