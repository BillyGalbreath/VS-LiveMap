export class Zoom {
    private readonly _def: number;
    private readonly _maxin: number;
    private readonly _maxout: number;

    constructor(def?: number, min?: number, max?: number) {
        this._def = def ?? 0;
        this._maxin = min ?? 2;
        this._maxout = max ?? 3;
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
}
