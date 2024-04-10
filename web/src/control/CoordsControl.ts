import * as L from 'leaflet';
import {ControlBox} from './ControlBox';
import {LiveMap} from '../LiveMap';
import {Util} from '../util/Util';

export class CoordsControl extends ControlBox {
    private readonly _dom: HTMLElement;

    private _x: number = 0;
    private _y: number = 0;

    constructor(livemap: LiveMap) {
        super(livemap, 'bottomleft');

        this._dom = L.DomUtil.create('div', 'leaflet-control-layers coordinates');

        // todo - why does this break lighthouse's accessibility report?? (axe-core Error: wf[r] is not a function)
        //this._dom.innerHTML = "0, 0";

        L.DomEvent.disableClickPropagation(this._dom);

        this.addTo(livemap);
    }

    onAdd(map: L.Map): HTMLElement {
        map.addEventListener('mousemove', this.update);
        return this._dom;
    }

    onRemove(map: L.Map): void {
        map.removeEventListener('mousemove', this.update);
    }

    public update = (e: L.LeafletMouseEvent): void => {
        const point: L.Point = Util.toPoint(e!.latlng);
        this._x = Math.round(point.x - this._livemap.settings.spawn.x);
        this._y = Math.round(point.y - this._livemap.settings.spawn.y);

        // todo - why does this break lighthouse's accessibility report?? (axe-core Error: wf[r] is not a function)
        this._dom.innerHTML = `${this._x}, ${this._y}`;
    }

    public getCoordinates(): L.PointTuple {
        return [this._x, this._y];
    }
}
