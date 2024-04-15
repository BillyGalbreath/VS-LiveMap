import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Location} from "../data/Location";

export class CoordsControl extends ControlBox {
    private readonly _dom: HTMLElement;
    private readonly _domX: HTMLElement;
    private readonly _domZ: HTMLElement

    private _loc: Location = new Location(0, 0);

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomcenter');

        this._dom = L.DomUtil.create('div', 'leaflet-control-layers coordinates');
        const p: HTMLElement = L.DomUtil.create('p', '', this._dom);
        this._domX = L.DomUtil.create('span', '', p);
        L.DomUtil.create('span', '', p).innerHTML = ',';
        this._domZ = L.DomUtil.create('span', '', p);

        L.DomEvent.disableClickPropagation(this._dom);

        this.addTo(livemap);
    }

    onAdd(map: L.Map): HTMLElement {
        map.addEventListener('mousemove', this.update);

        this.update();

        return this._dom;
    }

    onRemove(map: L.Map): void {
        map.removeEventListener('mousemove', this.update);
    }

    public update = (e?: L.LeafletMouseEvent): void => {
        this._loc = e
            ? Location.of(e.latlng)
                .subtract(this._livemap.settings.spawn)
                .round()
            : new Location(0, 0);
        this._domX.innerHTML = `${this._loc.x}`;
        this._domZ.innerHTML = `${this._loc.z}`;
    }

    public getLocation(): Location {
        return this._loc;
    }
}
