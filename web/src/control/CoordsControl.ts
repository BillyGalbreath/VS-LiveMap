import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Util} from '../util/Util';

export class CoordsControl extends ControlBox {
    private readonly _dom: HTMLDivElement;

    private _x: number = 0;
    private _y: number = 0;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('div', 'leaflet-control-layers coordinates');

        L.DomEvent.disableClickPropagation(this._dom);

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
        if (!e) {
            this._dom.innerHTML = '0, 0';
            return;
        }

        const point: L.Point = Util.toPoint(e!.latlng);
        this._x = Math.round(point.x - this._livemap.settings.spawn.x);
        this._y = Math.round(point.y - this._livemap.settings.spawn.y);

        this._dom.innerHTML = `${this._x}, ${this._y}`;
    }

    public getCoordinates(): L.PointTuple {
        return [this._x, this._y];
    }
}
