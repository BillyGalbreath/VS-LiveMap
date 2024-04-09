import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Util} from "../util/Util";
import '../svg/link.svg';

export class LinkControl extends ControlBox {
    private readonly _dom: HTMLAnchorElement;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('a', 'leaflet-control-layers link');
        this._dom.title = 'Share this location';
        this._dom.onclick = (e: MouseEvent): void => {
            e.preventDefault();
            window.history.replaceState({}, 'LiveMap', this._dom.href);
            navigator.clipboard.writeText(this._dom.href).then((): void => {
                // todo feedback popup
            });
        }

        L.DomEvent.disableClickPropagation(this._dom);

        this._dom.appendChild(Util.createSVGIcon('link'));
        //const img: HTMLImageElement = L.DomUtil.create('img', '', this._dom);
        //img.src = 'images/icon/link.svg';
        //img.alt = '';

        this.addTo(livemap);
    }

    onAdd(map: L.Map): HTMLAnchorElement {
        map.addEventListener('moveend', this.update);
        map.addEventListener('zoomend', this.update);
        return this._dom;
    }

    onRemove(map: L.Map): void {
        map.removeEventListener('moveend', this.update);
        map.removeEventListener('zoomend', this.update);
    }

    update = (): void => {
        this._dom.href = this._livemap.getUrlFromView();
    }
}
