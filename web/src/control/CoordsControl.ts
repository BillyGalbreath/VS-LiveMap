import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Location} from "../data/Location";

export class CoordsControl extends ControlBox {
    private readonly _dom: HTMLElement;

    private _loc: Location = new Location(0, 0);

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('div', 'leaflet-control-layers coordinates');

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
        if (!e) {
            this._dom.innerHTML = '0, 0';
            return;
        }

        this._loc = Location.of(e!.latlng)
            .subtract(this._livemap.settings.spawn)
            .round();
        this._dom.innerHTML = `${this._loc.x}, ${this._loc.z}`;
    }

    public getLocation(): Location {
        return this._loc;
    }
}
