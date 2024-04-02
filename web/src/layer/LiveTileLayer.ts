import * as L from "leaflet";
import {LiveMap} from "../LiveMap";

export class LiveTileLayer extends L.TileLayer {
    constructor(livemap: LiveMap) {
        super('tiles/{z}/{x}_{y}.png', {
            // tile sizes match regions sizes (512 blocks x 512 blocks)
            tileSize: 512,
            // dont wrap tiles at edges
            noWrap: true,
            // zoom stuff (this is a pita, btw)
            // this doesnt work right, so we leave it false and override _getZoomForUrl below
            zoomReverse: false,
            // the closest zoomed in possible (without stretching)
            // this is always 0. no exceptions!
            minNativeZoom: 0,
            // the farthest possible zoom out possible
            maxNativeZoom: livemap.settings.zoom.maxout,
            // the closest zoomed in possible (without stretching)
            // this is always 0. no exceptions!
            minZoom: 0,
            // for extra zoom in, make higher than maxNativeZoom
            // this is the stretched tiles to zoom in further
            // maxZoom = maxNativeZoom + extra
            // maxZoom = zoom.maxout + zoom.maxin
            maxZoom: livemap.settings.zoom.maxout + livemap.settings.zoom.maxin,
            // we need to counter effect the higher maxZoom here
            // zoomOffset = maxNativeZoom - maxZoom
            // zoomOffset = zoom.maxout - (zoom.maxout + zoom.maxin)
            // zoomOffset = -(zoom.maxin)
            zoomOffset: -livemap.settings.zoom.maxin
        });

        // push this layer to the back (leaflet defaults it to 1)
        this.setZIndex(0);
    }

    // reverse zoom controls here instead of the flag in options
    _getZoomForUrl(): number {
        return (this.options.maxZoom! - this._tileZoom!) + this.options.zoomOffset!;
    }

    // @method createTile(coords: Object, done?: Function): HTMLElement
    // Called only internally, overrides GridLayer's [`createTile()`](#gridlayer-createtile)
    // to return an `<img>` HTML element with the appropriate image URL given `coords`. The `done`
    // callback is called when the tile has been loaded.
    createTile(coords: L.Coords, done: L.DoneCallback): HTMLImageElement {
        const tile: HTMLImageElement = L.DomUtil.create('img');

        L.DomEvent.on(tile, 'load', L.Util.bind(this._tileOnLoad, this, done, tile));
        L.DomEvent.on(tile, 'error', L.Util.bind(this._tileOnError, this, done, tile));

        if (this.options.crossOrigin || this.options.crossOrigin === '') {
            tile.crossOrigin = '';
        }

        // Alt tag is set to empty string to keep screen readers from reading
        // URL and for compliance reasons http://www.w3.org/TR/WCAG20-TECHS/H67
        tile.alt = '';

        // Set role="presentation" to force screen readers to ignore this
        // https://www.w3.org/TR/wai-aria/roles#textalternativecomputation
        tile.setAttribute('role', 'presentation');

        // Retrieve image via a fetch instead of just setting the src
        // This works around the fact that browsers usually don't make a request
        // for an image that was previously loaded, without resorting to
        // changing the URL (which would break caching).
        fetch(this.getTileUrl(coords)).then((res: Response): void => {
            // Call leaflet's error handler if request fails for some reason
            if (!res.ok) {
                this._tileOnError(done, tile, new Error(res.statusText));
                return;
            }

            // Get image data and convert into object URL so it can be used as a src
            // Leaflet's onload listener will take it from here
            res.blob().then((blob: Blob): void => {
                // don't use URL.createObjectURL, it creates memory leak
                const reader: FileReader = new FileReader();
                reader.readAsDataURL(blob);
                reader.onload = () => tile.src = String(reader.result);
            });
        }).catch((e) => this._tileOnError(done, tile, e));

        return tile;
    }
}