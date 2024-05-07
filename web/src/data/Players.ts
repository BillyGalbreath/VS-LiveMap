import {Player} from './Player';

export class Players {
    private readonly _players: Player[];

    constructor(json?: Players) {
        this._players = json?.players ?? [];
    }

    get players(): Player[] {
        return this._players;
    }
}
