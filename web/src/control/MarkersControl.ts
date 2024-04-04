import * as L from "leaflet";
import {LiveMap} from "../LiveMap";
import {MarkersLayer} from "../layer/MarkersLayer";

export class MarkersControl {
    private readonly _control: L.Control.Layers;
    private readonly _layers: MarkersLayer[] = [];

    constructor(livemap: LiveMap) {
        this._control = L.control.layers({}, {}, {
            position: 'topleft'
        }).addTo(livemap);

        // todo - fetch list of json files to process
        // each json file is its own layer full of markers
        try {
            this._layers.push(new MarkersLayer(livemap, "markers/spawn.json"));
            //this._layers.push(new MarkersLayer(livemap, this, "markers/players.json"));
            this._layers.push(new MarkersLayer(livemap, "markers/test.json"));
        } catch (e) {
            console.error("ERRoR", e);
        }
    }

    public tick(count: number): void {
        this._layers.forEach((layer: MarkersLayer): void => {
            layer.tick(count);
        })
    }

    public addOverlay(layer: MarkersLayer): void {
        this._control.addOverlay(layer, layer.label);
    }
}
