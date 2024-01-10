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
            // show attribution control box
            attributionControl: true,
            // canvas is more efficient than svg
            preferCanvas: true
        }).on('overlayadd', (e) => {
            this.layerControls.showLayer(e.layer);
        }).on('overlayremove', (e) => {
            this.layerControls.hideLayer(e.layer);
        });

        // remove leaflet's attribution. we'll add our own in the tile layer
        this.map.attributionControl.setPrefix('');

        this.tick_count = 1;

        this.layerControls = new LayerControls();

        this.init();
    }

    init() {
        this.getJSON("tiles/settings.json", (json) => {
            this.layerControls.init();

            this.coords = new Coords(json.coords);
            this.link = new Link(json.link);

            this.zoom = json.zoom;
            this.spawn = json.spawn;
            this.marker_update_interval = json.marker_update_interval;
            this.tiles_update_interval = json.tiles_update_interval;
        });

        // move to the center of the map
        this.map.setView([512000, 512000], 0)
    }

    loop() {
        if (document.visibilityState === 'visible') {
            this.tick();
            this.tick_count++;
        }
        setTimeout(() => this.loop(), 1000);
    }

    tick() {
        // todo - tick player list
        // todo - tick tiles
    }

    toLatLng(x, z) {
        return L.latLng(this.pixelsToMeters(-z), this.pixelsToMeters(x));
    }

    toPoint(latlng) {
        return L.point(this.metersToPixels(latlng.lng), this.metersToPixels(-latlng.lat));
    }

    pixelsToMeters(num) {
        return num * this.scale;
    }

    metersToPixels(num) {
        return num / this.scale;
    }

    setScale(zoom) {
        this.scale = (1 / Math.pow(2, zoom));
    }

    getUrlFromView() {
        const center = this.toPoint(this.map.getCenter());
        const zoom = this.map.getZoom();
        const x = Math.floor(center.x);
        const z = Math.floor(center.y);
        return `?x=${x}&z=${z}&zoom=${zoom}`;
    }

    getJSON(url, fn) {
        fetch(url, {cache: "no-store"})
            .then(async res => {
                if (res.ok) {
                    fn(await res.json());
                }
            });
    }
}

export const LM = new LiveMap();

// https://stackoverflow.com/a/3955096
Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};

// tile layer extension
const tileLayer = L.TileLayer.extend({

    // tile layer options
    options: {
        // tiles are 512x512 blocks
        tileSize: 512,
        // do not wrap tiles around the antimeridian
        noWrap: true,
        // our custom attribution (link to project page)
        attribution: "<a href='https://mods.vintagestory.at/livemap' target='_blank'>Livemap</a> &copy; 2024",

        // zoom stuff (this is a pita, btw)
        //zoomReverse: false, // will this work instead of custom _getZoomForUrl below?
        //minNativeZoom: 0,
        //maxNativeZoom: 3,
        minZoom: 0,
        maxZoom: 3,
        //zoomOffset: -2
    },

    // better controls for zooming
    _getZoomForUrl: function () {
        return (this.options.maxZoom - this._tileZoom) + this.options.zoomOffset;
    }
});

// create the tile layer and add to map
//new tileLayer('tiles/{x}_{y}.png').addTo(map);
