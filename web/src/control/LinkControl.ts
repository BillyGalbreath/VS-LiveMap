import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Point} from "../data/Point";

export class LinkControl extends ControlBox {
    private readonly _dom: HTMLAnchorElement;

    private _basePath: string = '/';

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
        return this.getUrlFromPoint(
            Point.of(this._livemap.getCenter())
                .floor()
                .subtract(this._livemap.settings.spawn)
        );
    }

    public getUrlFromPoint(point: Point): string {
        const renderer: string = this._livemap.sidebarControl.renderersControl.rendererType;
        const zoom: number = this._livemap.currentZoom();
        if (this._livemap.settings.friendlyUrls) {
            return `${this._basePath}${renderer}/${zoom}/${point.x}/${point.z}/`;
        } else {
            return `${this._basePath}?renderer=${renderer}&zoom=${zoom}&x=${point.x}&z=${point.z}`;
        }
    }

    public centerOnUrl(): void {
        let renderer, zoom, x, z;
        const match: RegExpExecArray | null = /(.*\/)?(.+)\/([+-]?\d+)\/([+-]?\d+)\/([+-]?\d+)(\/.*)?$/.exec(window.location.pathname);
        if (match) {
            this._basePath = match[1] ?? '/';
            renderer = match[2];
            zoom = match[3];
            x = match[4];
            z = match[5];
        } else {
            const url: URLSearchParams = new URLSearchParams(window.location.search);
            this._basePath = window.location.pathname?.split('?')[0]?.replace('index.html', '') ?? '/';
            renderer = url.get('renderer');
            zoom = url.get('zoom');
            x = url.get('x');
            z = url.get('z');
        }
        this._livemap.sidebarControl.renderersControl.rendererType = renderer ?? this._livemap.settings.renderers[0].id;
        this._livemap.centerOn(Point.of(x ?? 0, z ?? 0), zoom ?? this._livemap.settings.zoom.def);
    }
}
