import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {PinControl} from "./PinControl";
import {PlayersControl} from "./PlayersControl";
import {RenderersControl} from "./RenderersControl";

export class SidebarControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;

    private readonly _renderersControl: RenderersControl;
    private readonly _playersControl: PlayersControl;

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._renderersControl = new RenderersControl(livemap);
        this._playersControl = new PlayersControl(livemap);

        this._dom = L.DomUtil.create('aside');
        document.body.prepend(this._dom);

        const pin: PinControl = new PinControl(livemap);
        if (pin.pinned) {
            this.show();
        } else {
            this.hide();
        }

        const div: HTMLDivElement = L.DomUtil.create('div', '', this._dom);
        div.appendChild(window.createSVGIcon("logo"));
        const span: HTMLSpanElement = L.DomUtil.create('span', '', div);
        span.innerText = "LiveMap";

        // add these after the pin
        this._dom.appendChild(this.renderersControl.dom);
        this._dom.appendChild(this.playersControl.dom);

        this.tick();

        this._dom.onclick = (): void => {
            // todo followPlayerMarker(null)
        };
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

    get renderersControl(): RenderersControl {
        return this._renderersControl;
    }

    get playersControl(): PlayersControl {
        return this._playersControl;
    }

    public hide(): void {
        this._dom.classList.remove('show');
    }

    public show(): void {
        this._dom.classList.add('show');
    }

    public tick(): void {
        this.playersControl.tick();
    }
}
