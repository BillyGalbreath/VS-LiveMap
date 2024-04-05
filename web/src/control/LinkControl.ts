import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';

export class LinkControl extends ControlBox {
    private readonly _dom: HTMLAnchorElement;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('a', 'leaflet-control-layers link');
        this._dom.title = 'Get link from view';

        L.DomEvent.disableClickPropagation(this._dom);

        const img: HTMLImageElement = L.DomUtil.create('img', '', this._dom);
        img.src = 'images/link.png';
        img.alt = '';

        this.addTo(livemap);
    }

    onAdd(map: L.Map): HTMLAnchorElement {
        map.addEventListener('moveend', this.update);
        map.addEventListener('zoomend', this.update);

        this.update();

        return this._dom;
    }

    onRemove(map: L.Map): void {
        map.removeEventListener('moveend', this.update);
        map.removeEventListener('zoomend', this.update);
    }

    public update = (): void => {
        this._dom.href = this._livemap.getUrlFromView();
    }
}
