import * as L from "leaflet";

window.onload = function (): void {
    window.livemap = new Livemap();
};

export class Livemap {
    private readonly _map: L.Map;

    constructor() {
        this._map = L.map('map', {
            crs: L.Util.extend(L.CRS.Simple, {
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            center: [0, 0],//[this.mapSizeX / 2, this.mapSizeZ / 2],
            attributionControl: true,
            preferCanvas: true,
            zoomSnap: 1 / 4,
            zoomDelta: 1 / 4,
            wheelPxPerZoomLevel: 60 * 4
        }).on('overlayadd', (e: L.LayersControlEvent): void => {
            //this.layerControls.showLayer(e.layer);
        }).on('overlayremove', (e: L.LayersControlEvent): void => {
            //this.layerControls.hideLayer(e.layer);
        });

        this._map.attributionControl.setPrefix('<a href="https://mods.vintagestory.at/livemap" target="_blank">Vintage Story Livemap</a> &copy;2024');
    }
}
