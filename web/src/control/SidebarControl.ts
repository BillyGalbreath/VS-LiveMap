import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {PinControl} from "./PinControl";

export class SidebarControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;

    private readonly _playersLegend: HTMLElement

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._dom = L.DomUtil.create('div', '', document.body);
        this._dom.id = 'sidebar';

        this._dom.onclick = (): void => {
            // todo followPlayerMarker(null)
        };

        const pin: PinControl = new PinControl(livemap);
        if (pin.pinned) {
            this.show();
        } else {
            this.hide();
        }

        const fieldRenderers: HTMLFieldSetElement = L.DomUtil.create('fieldset', 'renderers', this._dom);
        L.DomUtil.create('legend', '', fieldRenderers).textContent = livemap.settings.lang.renderers;

        const fieldPlayers: HTMLFieldSetElement = L.DomUtil.create('fieldset', 'players', this._dom);
        this._playersLegend = L.DomUtil.create('legend', '', fieldPlayers);

        this.tick(0);

        this._dom.onmouseleave = (): void => {
            if (!pin.pinned) {
                this.hide();
            }
        };
        this._dom.onmouseenter = (): void => {
            if (!pin.pinned) {
                this.show();
            }
        };
    }

    public hide(): void {
        this._dom.classList.remove('show');
    }

    public show(): void {
        this._dom.classList.add('show');
    }

    public tick(_: number): void {
        this._playersLegend.textContent = this._livemap.settings.lang.players
            .replace(/{cur}/g, this._livemap.playersControl.cur.toString())
            .replace(/{max}/g, this._livemap.playersControl.max.toString())
    }
}
