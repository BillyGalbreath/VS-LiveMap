import {LM} from '../livemap.js';
import {LiveMapTileLayer} from './livemaptilelayer.js';

export class LayerControls {
    constructor() {
        this.layers = new Map();
    }

    init() {
        this.currentLayer = 0;
        this.updateInterval = 60;

        this.playersLayer = new L.LayerGroup();
        this.playersLayer.id = "players_layer";

        this.controls = L.control.layers({}, {}, {
            position: 'topleft',
            sortLayers: true,
            sortFunction: (a, b) => {
                return a.order - b.order;
            }
        });

        this.controls.addTo(LM.map);

        this.setupTileLayers();
    }

    addOverlay(name, layer, hide) {
        this.controls.addOverlay(layer, name);
        if (this.shouldHide(layer, hide) !== true) {
            layer.addTo(LM.map);
        }
    }

    removeOverlay(layer) {
        this.ignoreLayer = layer;
        this.controls.removeLayer(layer);
        layer.remove();
        this.ignoreLayer = null;
    }

    shouldHide(layer, def) {
        const value = window.localStorage.getItem(`hide_${layer.id}`);
        return value == null ? def : value === 'true';
    }

    hideLayer(layer) {
        if (layer != this.ignoreLayer) {
            window.localStorage.setItem(`hide_${layer.id}`, 'true');
        }
    }

    showLayer(layer) {
        if (layer != this.ignoreLayer) {
            window.localStorage.setItem(`hide_${layer.id}`, 'false');
        }
    }

    setupTileLayers() {
        // setup the map tile layers
        // we need 2 layers to swap between for seamless refreshing
        if (this.tileLayer1 != null) {
            LM.map.removeLayer(this.tileLayer1);
        }
        if (this.tileLayer2 != null) {
            LM.map.removeLayer(this.tileLayer2);
        }
        this.tileLayer1 = this.createTileLayer();
        this.tileLayer2 = this.createTileLayer();
    }

    createTileLayer() {
        return new LiveMapTileLayer(`tiles/{z}/{x}_{y}.png`)
            .addTo(LM.map)
            .addEventListener("load", () => {
                // when all tiles are loaded, switch to this layer
                this.switchTileLayer();
            });
    }

    updateTileLayer() {
        // redraw background tile layer
        if (this.currentLayer == 1) {
            this.tileLayer2.redraw();
        } else {
            this.tileLayer1.redraw();
        }
    }

    switchTileLayer() {
        // swap current tile layer
        if (this.currentLayer == 1) {
            this.tileLayer1.setZIndex(0);
            this.tileLayer2.setZIndex(1);
            this.currentLayer = 2;
        } else {
            this.tileLayer1.setZIndex(1);
            this.tileLayer2.setZIndex(0);
            this.currentLayer = 1;
        }
    }
}
