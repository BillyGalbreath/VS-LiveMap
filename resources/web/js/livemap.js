import {Coords} from "./modules/coords.js";
import {LayerControls} from "./modules/layercontrols.js";
import {Link} from "./modules/link.js";

class LiveMap {
    constructor() {
        // create the map
        this.map = L.map('map', {
            // we need a flat and simple crs
            crs: L.Util.extend(L.CRS.Simple, {
                // we need to flip the y-axis correctly
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            // always 0,0 center
            center: [0, 0],
            // show attribution control box
            attributionControl: true,
            // canvas is more efficient than svg
            preferCanvas: true,
            zoomSnap: 1 / 4,
            zoomDelta: 1 / 4,
            wheelPxPerZoomLevel: 60 * 4
        }).on('overlayadd', (e) => {
            this.layerControls.showLayer(e.layer);
        }).on('overlayremove', (e) => {
            this.layerControls.hideLayer(e.layer);
        });

        // replace leaflet's attribution with our own
        this.map.attributionControl.setPrefix(
            `<a href="https://mods.vintagestory.at/livemap" target="_blank">Vintage Story Livemap</a>
             &copy; 2024 <sup><a href="https://github.com/billygalbreath/vs-livemap" target="_blank">MIT</a></sup>`
        );

        this.tick_count = 1;

        this.layerControls = new LayerControls(this.map);

        this.getJSON("tiles/settings.json", (json) => {
            // set the scale for our projection calculations
            this.scale = (1 / Math.pow(2, json.zoom?.max ?? 3));

            // move to the center of the map at default zoom level
            this.centerOn(
                this.getUrlParam("x", json.spawn?.x ?? 0),
                this.getUrlParam("z", json.spawn?.z ?? 0),
                this.getUrlParam("y", json.zoom?.def ?? 0)
            );

            // setup the layer controls (tile layers and layer overlays)
            this.layerControls.setupLayers();

            // setup other control boxes
            this.coords = new Coords(json.coords ?? true);
            this.link = new Link(json.link ?? true);
        });
    }

    loop() {
        this.tick_count++;

        if (document.visibilityState === 'visible') {
            this.tick();
        }

        setTimeout(() => this.loop(), 1000);
    }

    tick() {
        // todo - tick player list
        // todo - tick tiles
    }

    centerOn(x, z, zoom) {
        this.map.setView(this.toLatLng(x, z), 3 - zoom);
        this.link?.update();
    }

    toLatLng(x, z) {
        return L.latLng(z * this.scale, x * this.scale);
    }

    toPoint(latlng) {
        return L.point(latlng.lng / this.scale, latlng.lat / this.scale);
    }

    getJSON(url, fn) {
        fetch(url, {cache: "no-store"})
            .then(async res => {
                if (res.ok) {
                    fn(await res.json());
                }
            });
    }

    getUrlParam(query, def) {
        return new URLSearchParams(window.location.search).get(query) ?? def;
    }

    getUrlFromView() {
        const center = this.toPoint(this.map.getCenter());
        const zoom = 3 - this.map.getZoom();
        const x = Math.floor(center.x);
        const z = Math.floor(center.y);
        return `?x=${x}&z=${z}&y=${zoom}`;
    }
}

export const LM = new LiveMap();

// https://stackoverflow.com/a/3955096
Array.prototype.remove = function () {
    let what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};
