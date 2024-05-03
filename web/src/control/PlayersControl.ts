import {LiveMap} from "../LiveMap";
import {Players} from "../data/Players";
import {Player} from "../data/Player";
import * as L from "leaflet";

export class PlayersControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;

    private _players: Players = new Players();

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._dom = L.DomUtil.create('ul');

        // todo - temporary
        this.tick2();
    }

    get dom(): HTMLElement {
        return this._dom;
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
    }

    public tick2(): void {
        window.fetchJson<Players>('data/players.json')
            .then((json: Players): void => {
                this._players = json;
                this.players.forEach((player: Player): void => {
                    this._dom.appendChild(this.createPlayer(player));
                })
            })
            .catch((err: unknown): void => {
                console.error(`Error loading players list data\n`, err);
            });
        /*this._legend.textContent = this._livemap.settings.lang.players
            .replace(/{cur}/g, this.cur.toString())
            .replace(/{max}/g, this.max.toString());/*/
    }

    private createPlayer(player: Player): HTMLLIElement {
        const li: HTMLLIElement = L.DomUtil.create('li');
        li.id = player.id;
        li.title = player.name;

        const img: HTMLImageElement = L.DomUtil.create('img', '', li);
        img.src = player.avatar;
        img.alt = player.name;

        const p: HTMLParagraphElement = L.DomUtil.create('p', '', li);
        p.innerText = player.name;

        return li;
    }
}
