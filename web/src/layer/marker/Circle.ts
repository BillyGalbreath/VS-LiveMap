import * as L from 'leaflet';
import {Marker, MarkerJson} from './Marker';
import {MarkersLayer} from '../MarkersLayer';
import {Point} from '../../data/Point';

export class Circle extends Marker {
    constructor(layer: MarkersLayer, json: MarkerJson) {
        super(layer, json, L.circle(Point.of(json.point).toLatLng(), {
            ...json.options,
            radius: Circle.radius(json)
        }));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        (this._marker as L.Circle)
            .setLatLng(Point.of(json.point).toLatLng())
            .setRadius(Circle.radius(json));
    }

    private static radius(json: MarkerJson): number {
        return Point.pixelsToMeters((json.options as L.CircleOptions).radius ?? 10);
    }
}
