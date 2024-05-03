import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Point} from "../data/Point";

export class LinkControl extends ControlBox {
    private readonly _dom: HTMLAnchorElement;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('a', 'leaflet-control-layers link');
        this._dom.title = 'Share this location';
        this._dom.appendChild(window.createSVGIcon('link'));
        this._dom.onclick = (e: MouseEvent): void => {
            e.preventDefault();
            window.history.replaceState({}, 'LiveMap', this._dom.href);
            this._livemap.contextMenu.share(
                Point.of(this._livemap.getCenter())
                    .floor()
                    .subtract(this._livemap.settings.spawn)
            );
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
        this._dom.href = this.getUrlFromView();

        // todo - find out how to prevent chrome from spamming history
        window.history.replaceState({}, '', this._dom.href);
    }

    public getUrlFromView(): string {
        const point: Point = Point.of(this._livemap.getCenter())
            .floor()
            .subtract(this._livemap.settings.spawn);
        return `?renderer=${this._livemap.sidebarControl.renderersControl.rendererType}&x=${point.x}&z=${point.z}&zoom=${this._livemap.currentZoom()}`;
    }
}
