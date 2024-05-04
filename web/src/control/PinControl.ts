import * as L from 'leaflet';
import {LiveMap} from "../LiveMap";
import {Lang} from "../data/Lang";

export class PinControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;
    private readonly _img: HTMLImageElement;

    private _pinned: boolean = false;

    constructor(livemap: LiveMap, parent: HTMLElement) {
        this._livemap = livemap;

        this._dom = L.DomUtil.create('div');
        this._dom.id = 'pin';
        this._dom.onclick = (): void => {
            this.pin(!this.pinned);
            localStorage.setItem("pinned", this.pinned ? "pinned" : "unpinned");
        };

        if (livemap.settings.ui.sidebar.pinned != "hide") {
            parent.appendChild(this._dom);
        }

        this._img = L.DomUtil.create('img', '', this._dom);

        this.pin("pinned" === (localStorage.getItem("pinned") ?? livemap.settings.ui.sidebar.pinned));
    }

    public get pinned(): boolean {
        return this._pinned;
    }

    public pin(pinned: boolean): void {
        this._pinned = pinned;

        const lang: Lang = this._livemap.settings.lang;
        const state: string = pinned ? 'pinned' : 'unpinned';
        const text: string = eval(`lang.${state}`);

        this._dom.className = state;
        this._img.src = `images/${state}.png`;
        this._img.alt = text;
        this._img.title = text;
    }
}
