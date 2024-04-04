import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Util} from "../util/Util";

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
        if (!Util.isset(e)) {
            this._dom.innerHTML = '0, 0';
            return;
        }

        let point: L.Point = Util.toPoint(e!.latlng);
        let x: number = Math.round(point.x - this._livemap.settings.spawn.x);
        let y: number = Math.round(point.y - this._livemap.settings.spawn.y);

        this._dom.innerHTML = `${x}, ${y}`;
    }
}
