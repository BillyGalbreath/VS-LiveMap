import {LiveMap} from "../LiveMap";
import {LiveTileLayer} from "./LiveTileLayer";


export class LayerControl {
    private readonly _layers: LiveTileLayer[] = [];

    private _cur: boolean = false;

    constructor(livemap: LiveMap) {
        // we need 2 tile layers to swap between for seamless refreshing
        livemap.addLayer(this._layers[0] = this.createTileLayer(livemap));
        livemap.addLayer(this._layers[1] = this.createTileLayer(livemap));

        // todo - add overlay layers
        // players
        // traders
        // translocators
        // custom layers
    }

    private createTileLayer(livemap: LiveMap): LiveTileLayer {
        return new LiveTileLayer(livemap)
            .addEventListener("load", (): void => {
                // switch layers when all tiles are loaded
                this.switchTileLayer();
            });
    }

    private switchTileLayer(): void {
        // swap current tile layer
        this._layers[+this._cur].setZIndex(0);
        this._layers[+!this._cur].setZIndex(1);
        this._cur = !this._cur;
    }

    public updateTileLayer(): void {
        // redraw opposite tile layer
        // it will switch to it when all tiles load
        this._layers[+!this._cur].redraw();
    }
}
