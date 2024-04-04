import * as L from "leaflet";
import {LiveMap} from "../LiveMap";
import {Marker, MarkersLayer} from "../layer/MarkersLayer";
import {Util} from "../util/Util";

export interface Layer {
    label: string,
    interval: number,
    markers: Marker[];
}

export class MarkersControl {
    private readonly _control: L.Control.Layers;
    private readonly _layers: MarkersLayer[] = [];

    constructor(livemap: LiveMap) {
        this._control = L.control.layers({}, {}, {
            position: 'topleft'
        }).addTo(livemap);

        // todo - fetch list of json files to process
        // each json file is its own layer full of markers
        Util.fetchJson("markers/spawn.json").then((layer: Layer): void => {
            try {
                const l: MarkersLayer = new MarkersLayer(livemap, layer.markers, layer.interval);
                this._layers.push(l);
                this._control.addOverlay(l, layer.label);
            } catch (e) {
                console.error("ERRoR", e);
            }
        });
    }

    public tick(count: number): void {
        this._layers.forEach((layer: MarkersLayer): void => {
            layer.tick(count);
        })
    }
}
