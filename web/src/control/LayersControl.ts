import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {MarkersLayer} from '../layer/MarkersLayer';

export class LayersControl extends L.Control.Layers {
    declare _layers: L.Control.LayersObject[];

    constructor(livemap: LiveMap) {
        super({}, {}, {
            position: 'topleft'
        });

        this.addTo(livemap);

        // each json file is its own layer full of markers
        livemap.settings.markers?.forEach((layer: string): void => {
            try {
                new MarkersLayer(livemap, `data/markers/${layer}.json`);
            } catch (e) {
                console.error(`Error loading markers layer (${layer})\n`, e);
            }
        });
    }

    public tick(count: number): void {
        this._layers.forEach((obj: L.Control.LayersObject): void => {
            try {
                (obj.layer as MarkersLayer).tick(count);
            } catch (e) {
                console.error('Error ticking markers layer\n', obj.layer, e);
            }
        });
    }
}
