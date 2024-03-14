export const ReverseZoomTileLayer = L.TileLayer.extend({
    options: {
        // tiles are 512x512 blocks
        tileSize: 512,

        // do not wrap tiles around the antimeridian
        noWrap: true,

        // zoom stuff (this is a pita, btw)
        // this doesnt work right, so we leave it false and override _getZoomForUrl below
        zoomReverse: false,

        // the closest zoomed in possible (without extra/stretching)
        // this is always 0. no exceptions!
        minNativeZoom: 0,

        // the farthest rendered tiles zoom out level 
        maxNativeZoom: 0,

        // this is always 0. no exceptions!
        minZoom: 0,

        // for extra/stretching zoom in
        // maxNativeZoom + extra zoom = maxZoom
        maxZoom: 0 + 2,

        // we need to counter effect the higher maxZoom here
        // maxZoom + zoomOffset = maxNativeZoom
        zoomOffset: -2
    },

    // reverse zoom controls
    _getZoomForUrl: function () {
        return (this.options.maxZoom - this._tileZoom) + this.options.zoomOffset;
    },

    // @method createTile(coords: Object, done?: Function): HTMLElement
    // Called only internally, overrides GridLayer's [`createTile()`](#gridlayer-createtile)
    // to return an `<img>` HTML element with the appropriate image URL given `coords`. The `done`
    // callback is called when the tile has been loaded.
    createTile: function (coords, done) {
        const tile = document.createElement('img');

        L.DomEvent.on(tile, 'load', () => this._tileOnLoad(done, tile));
        L.DomEvent.on(tile, 'error', (e) => this._tileOnError(done, tile, e));

        if (this.options.crossOrigin || this.options.crossOrigin === '') {
            tile.crossOrigin = this.options.crossOrigin === true ? '' : this.options.crossOrigin;
        }

        tile.alt = '';
        tile.setAttribute('role', 'presentation');

        // retrieve image via a fetch instead of just setting the src
        // this works around the fact that browsers usually don't make
        // a request for an image that was previously loaded, without
        // resorting to changing the URL (which would break caching).
        fetch(this.getTileUrl(coords))
            .then(res => {
                //Call leaflet's error handler if request fails for some reason
                if (!res.ok) {
                    this._tileOnError(done, tile, new Error(res.statusText));
                    return;
                }

                // Get image data and convert into object URL, so it can be used as a src
                // Leaflet's onload listener will take it from here
                //res.blob().then(blob => tile.src = URL.createObjectURL(blob));
                res.blob().then((blob) => {
                    // don't use URL.createObjectURL, it creates memory leak
                    const reader = new FileReader();
                    reader.readAsDataURL(blob);
                    reader.onload = () => tile.src = String(reader.result);
                });
            }).catch((e) => this._tileOnError(done, tile, e));

        return tile;
    }
});
