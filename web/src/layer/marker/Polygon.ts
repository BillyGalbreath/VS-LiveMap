import * as L from 'leaflet';
import {Marker, MarkerJson} from './Marker';
import {MarkersLayer} from '../MarkersLayer';
import {Point} from '../../data/Point';

export class Polygon extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.polygon(Point.toLatLngArray(json.points) as L.LatLng[], json.options));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Polygon).setLatLngs(Point.toLatLngArray(json.points) as L.LatLng[]);
    }
}
