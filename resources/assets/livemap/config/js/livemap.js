import {Coords} from "./modules/coords.js";
import {LayerControls} from "./modules/layercontrols.js";
import {Link} from "./modules/link.js";

class LiveMap {
    constructor() {
        // defaults (will be overridden from settings.json)
        this.mapSizeX = 1024000;
        this.mapSizeZ = 1024000;
        this.spawnX = 512000;
        this.spawnZ = 512000;

        // create the map
        this.map = L.map('map', {
            // we need a flat and simple crs
            crs: L.Util.extend(L.CRS.Simple, {
                // we need to flip the y-axis correctly
                // https://stackoverflow.com/a/62320569/3530727
                transformation: new L.Transformation(1, 0, 1, 0)
            }),
            // always 0,0 center
            center: [this.mapSizeX / 2, this.mapSizeZ / 2],
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
            // setup the zoom levels
            this.zoomDef = json.zoom?.def ?? 0;
            this.zoomMax = json.zoom?.max ?? 3;

            // set the scale for our projection calculations
            this.scale = (1 / Math.pow(2, this.zoomMax));

            // set the world size and spawn point
            this.mapSizeX = json.mapsize?.x ?? 1024000;
            this.mapSizeZ = json.mapsize?.z ?? 1024000;
            this.spawnX = json.spawn?.x ?? (this.mapSizeX / 2);
            this.spawnZ = json.spawn?.z ?? (this.mapSizeZ / 2);

            // move to the coords or spawn point at specified or default zoom level
            this.centerOn(
                parseInt(this.getUrlParam("x", 0)),
                parseInt(this.getUrlParam("z", 0)),
                parseInt(this.getUrlParam("y", 0))
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
        this.map.setView(this.toLatLng(x + this.spawnX, z + this.spawnZ), 3 - zoom);
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
        const zoom = this.zoomMax - this.map.getZoom();
        const x = Math.floor(center.x) - this.spawnX;
        const z = Math.floor(center.y) - this.spawnZ;
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
