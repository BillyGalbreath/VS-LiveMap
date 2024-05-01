import {LiveMap} from "../LiveMap";
import {Players} from "../data/Players";
import {Player} from "../data/Player";

export class PlayersControl {
    private readonly _livemap: LiveMap;

    private _players: Players = new Players();

    constructor(livemap: LiveMap) {
        this._livemap = livemap;
    }

    get cur(): number {
        return this._players.list.length;
    }

    get max(): number {
        return this._players.max;
    }

    get players(): Player[] {
        return this._players.list;
    }

    public tick(): void {
        window.fetchJson<Players>('data/players.json')
            .then((json: Players): void => {
                this._players = json;
            })
            .catch((err: unknown): void => {
                console.error(`Error loading players list data\n`, err);
            });
    }
}
