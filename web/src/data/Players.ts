import {Player} from "./Player";

export class Players {
    private readonly _max: number;
    private readonly _list: Player[];

    constructor(players?: Players) {
        this._max = players?.max ?? 0;
        this._list = players?.list ?? [];
    }

    get max(): number {
        return this._max;
    }

    get list(): Player[] {
        return this._list;
    }
}
