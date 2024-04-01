export class Interval {
    private readonly _tiles: number;
    private readonly _players: number;
    private readonly _markers: number;

    constructor(tiles?: number, players?: number, markers?: number) {
        this._tiles = tiles ?? 30;
        this._players = players ?? 1;
        this._markers = markers ?? 60;
    }

    get tiles(): number {
        return this._tiles;
    }

    get players(): number {
        return this._players;
    }

    get markers(): number {
        return this._markers;
    }
}
