import * as L from 'leaflet';
import {LiveMap} from '../LiveMap';
import {Util} from '../util/Util';
import {Marker, MarkerJson, PopupOptions} from "./marker/Marker";
import {Circle} from "./marker/Circle";
import {Ellipse} from "./marker/Ellipse";
import {Icon} from "./marker/Icon";
import {Polygon} from "./marker/Polygon";
import {Polyline} from "./marker/Polyline";
import {Rectangle} from "./marker/Rectangle";

interface Defaults {
    options: L.LayerOptions,
    popup: PopupOptions,
    tooltip: L.TooltipOptions
}

interface LayerJson {
    label: string,
    interval: number,
    hidden: boolean,
    defaults: Defaults,
    options: L.LayerOptions,
    markers: MarkerJson[];
}

export class MarkersLayer extends L.LayerGroup {
    private readonly _livemap: LiveMap;
    private readonly _url: string;

    private readonly _markers: Map<string, Marker> = new Map<string, Marker>();

    private _label?: string;
    private _interval?: number;
    private _defaults?: Defaults;

    constructor(livemap: LiveMap, url: string) {
        super([]);
        this._livemap = livemap;
        this._url = url;

        // initial update to get interval and label
        this.updateLayer();
    }

    public tick(count: number): void {
        if (count % (this._interval ?? 0) == 0) {
            this.updateLayer();
        }
    }

    private initial(json: LayerJson): void {
        this._label = json.label;
        this._interval = json.interval ?? 300;
        this._defaults = json.defaults;

        // merge in the custom layer options
        if (json.options) {
            this.options = {
                ...this.options,
                ...json.options
            };
        }

        // only add to the map if we are not hiding it by default
        if (!json.hidden) {
            // adding to the map makes it visible
            this.addTo(this._livemap);
        }

        // add this layer to the layers control
        this._livemap.layersControl.addOverlay(this, this._label);
    }

    private updateLayer(): void {
        Util.fetchJson(this._url).then((json: LayerJson): void => {
            try {
                if (!this._label) {
                    // this is the first tick
                    this.initial(json);
                }

                // refresh markers
                this.updateMarkers(json);
            } catch (e) {
                console.error(`Error updating markers layer (${this._label})\n`, this, e);
            }
        });
    }

    private updateMarkers(json: LayerJson): void {
        const toRemove: string[] = [...this._markers.keys()];

        // get all markers from json
        json.markers.forEach((data: MarkerJson): void => {
            try {
                const marker: Marker | undefined = this._markers.get(data.id);
                if (marker) {
                    // update existing marker
                    marker.update(data);
                    toRemove.remove(data.id);
                } else {
                    // create new marker
                    this.createMarker(data);
                }
            } catch (e) {
                console.error(`Error refreshing markers in layer (${this._label})\n`, this, data, e);
            }
        });

        // any markers left over are no longer in the json file and must be removed
        toRemove.forEach((key: string): void => {
            this._markers.get(key)?.remove();
            this._markers.delete(key);
        });
    }

    private createMarker(data: MarkerJson): void {
        // merge in default options, if any
        if (this._defaults) {
            if (this._defaults.options) {
                data.options = {...data.options, ...this._defaults.options};
            }
            if (this._defaults.popup) {
                data.popup = {...data.popup, ...this._defaults.popup};
            }
            if (this._defaults.tooltip) {
                data.tooltip = {...data.tooltip, ...this._defaults.tooltip};
            }
        }

        // create any panes needed for this marker
        this._livemap.getOrCreatePane(data.options?.pane);
        this._livemap.getOrCreatePane(data.popup?.pane);
        this._livemap.getOrCreatePane(data.tooltip?.pane);

        // create new marker from data
        const marker: Marker = this.createType(this._livemap, data);

        // add marker to markers layer
        marker.addTo(this);

        // store marker
        this._markers.set(data.id, marker);
    }

    public createType(livemap: LiveMap, data: MarkerJson): Marker {
        switch (data.type) {
            case 'circle':
                return new Circle(livemap, data);
            case 'ellipse':
                return new Ellipse(livemap, data);
            case 'icon':
                return new Icon(livemap, data);
            case 'polygon':
                return new Polygon(livemap, data);
            case 'polyline':
                return new Polyline(livemap, data);
            case 'rectangle':
                return new Rectangle(livemap, data);
        }
        throw new Error(`Invalid marker type (${data.type})`);
    }
}
