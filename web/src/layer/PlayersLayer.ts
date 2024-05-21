import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {MarkersLayer} from './MarkersLayer';
import {Players} from '../data/Players';
import {Player} from '../data/Player';
import {Value} from '../data/Value';
import {Icon} from './marker/Icon';
import {Marker, MarkerJson} from './marker/Marker';

export class PlayersLayer extends MarkersLayer {
    private readonly _dom: HTMLElement;

    private readonly _players: Map<string, Player> = new Map();

    private _max: number = 0;

    constructor(livemap: LiveMap) {
        super(livemap, '', 1);
        this._dom = L.DomUtil.create('ul');

        this.options = {
            ...this.options,
            pane: 'players'
        };

        // add this layer to the layers control and map
        livemap.layersControl.addOverlay(this, livemap.settings.lang.players);
        livemap.addLayer(this);
    }

    get dom(): HTMLElement {
        return this._dom;
    }

    get cur(): number {
        return this._players.size;
    }

    get max(): number {
        return this._max;
    }

    get players(): IterableIterator<Player> {
        return this._players.values();
    }

    protected override updateLayer(): void {
        window.fetchJson<Players>('data/players.json')
            .then((json: Players): void => {
                this.updatePlayers(json);
            })
            .catch((err: unknown): void => {
                console.error(`Error loading players list data\n`, err);
            });

        // todo update player counts in sidebar legend
        /*this._legend.textContent = this._livemap.settings.lang.players
            .replace(/{cur}/g, this.cur.toString())
            .replace(/{max}/g, this.max.toString());/*/

        // todo follow highlighted player
    }

    private updatePlayers(json: Players): void {
        const toRemove: Set<string> = new Set(this._players.keys());

        json.players.forEach((data: Player): void => {
            const player: Player | undefined = this._players.get(data.name);
            if (player) {
                this.updatePlayer(player, new Player(data));
                toRemove.delete(player.name);
            } else {
                this.createPlayer(new Player(data));
            }
        });

        toRemove.forEach((name: string): void => {
            this.removePlayer(name);
        });
    }

    private createPlayer(player: Player): void {
        this._players.set(player.name, player);
        if (this._livemap.settings.playerMarkers) {
            this.addMarker(player);
        }
        if (this._livemap.settings.playerList) {
            this.addToSidebar(player);
        }
    }

    private updatePlayer(player: Player, data: Player): void {
        player.updateData(data);
        const icon: Icon = this.markers.get(player.name) as Icon;
        const marker: L.Marker = icon.get() as L.Marker;
        marker.setLatLng(data.pos.toLatLng());
        marker.setTooltipContent(this.tooltip(data));

        // https://stackoverflow.com/a/53416030/3530727
        const from: number = marker.options.rotationAngle ?? Math.round(data.yaw);
        const to: number = Math.round(data.yaw);
        const diff: number = ((((to - from) % 360) + 540) % 360) - 180;
        marker.options.rotationAngle = from + diff;
    }

    private removePlayer(name: string): void {
        this._players.delete(name);
        this.removeMarker(name);
        this.removeFromSidebar(name);
    }

    private addMarker(player: Player): void {
        const icon: Icon = new Icon(this, {
            type: 'icon',
            id: player.name,
            point: player.pos,
            options: {
                title: player.name,
                rotationAngle: player.yaw,
                iconUrl: '#svg-player',
                iconSize: [24, 24],
                iconAnchor: [12, 12]
            },
            tooltip: {
                'permanent': true,
                'direction': 'right',
                'offset': [10, 0],
                'pane': 'players',
                'content': this.tooltip(player)
            }
        } as unknown as MarkerJson);
        icon.addTo(this);
        this.markers.set(player.name, icon);
    }

    private removeMarker(name: string): void {
        const marker: Marker | undefined = this.markers.get(name);
        if (marker) {
            this.markers.delete(name)
            marker.remove();
        }
    }

    private addToSidebar(player: Player): void {
        const li: HTMLElement = L.DomUtil.create('li', '', this._dom);
        li.id = player.name;
        li.title = player.name;

        const img: HTMLImageElement = L.DomUtil.create('img', '', li);
        img.src = player.avatar;
        img.alt = `${player.name}'s Avatar`;

        const p: HTMLParagraphElement = L.DomUtil.create('p', '', li);
        p.innerText = player.name;
    }

    private removeFromSidebar(name: string): void {
        this._dom.querySelectorAll(`#${name}`)?.forEach((li: Element): void => li.remove());
    }

    private tooltip(player: Player): string {
        return `<ul><li><img src='${player.avatar}' alt='avatar'></li><li>${player.name}<div style='${this.stat(player.health, 3)}'><p></p></div><div style='${this.stat(player.satiety, 300)};--height:6px'><p></p></div></li></ul>`
    }

    private stat(value: Value, divisor: number): string {
        const segments: number = Math.round(value.max / divisor);
        const amount: number = Math.round((Math.min(value.cur, value.max) / value.max) * 100);
        return `--amount:${amount}%;--segments:${segments}`;
    }
}
