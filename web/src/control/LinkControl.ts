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
        this._dom.appendChild(Util.createSVGIcon('link'));
        this._dom.onclick = (e: MouseEvent): void => {
            e.preventDefault();
            window.history.replaceState({}, 'LiveMap', this._dom.href);

            const center: L.LatLng = this._livemap.getCenter();
            this._livemap.contextMenu.share([
                Math.floor(Util.metersToPixels(center.lat)) - this._livemap.settings.spawn.x,
                Math.floor(Util.metersToPixels(center.lng)) - this._livemap.settings.spawn.y
            ]);
        }

        L.DomEvent.disableClickPropagation(this._dom);

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

        // todo - find out how to prevent chrome from spamming history
        window.history.replaceState({}, '', this._dom.href);
    }
}
