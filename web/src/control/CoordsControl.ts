import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Point} from "../util/Point";

export class CoordsControl extends ControlBox {
    private readonly _dom: HTMLDivElement;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('div', 'leaflet-control-layers coordinates');

        this.addTo(livemap);
    }

    onAdd(map: L.Map): HTMLDivElement {
        map.addEventListener('mousemove', this.update);

        this.update();

        return this._dom;
    }

    onRemove(map: L.Map): void {
        map.removeEventListener('mousemove', this.update);
    }

    public update = (e?: L.LeafletMouseEvent): void => {
        if (!LiveMap.isset(e)) {
            this._dom.innerHTML = '0, 0';
            return;
        }

        let point: Point = this._livemap.toPoint(e!.latlng);
        let x: number = Math.round(point.x - this._livemap.spawn.x);
        let z: number = Math.round(point.z - this._livemap.spawn.z);

        this._dom.innerHTML = `${x}, ${z}`;
    }
}
