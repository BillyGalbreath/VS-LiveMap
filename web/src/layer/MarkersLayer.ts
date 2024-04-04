import * as L from "leaflet";
import {LiveMap} from "../LiveMap";
import {Util} from "../util/Util";

interface Layer {
    label: string,
    interval: number,
    hidden: boolean,
    options: L.LayerOptions,
    markers: Marker[];
}

interface Marker {
    type: string,
    point: L.PointExpression,
    options: L.MarkerOptions,
    tooltip: L.TooltipOptions,
    popup: L.PopupOptions;
}

export class MarkersLayer extends L.LayerGroup {
    private static readonly _types: {
        circle: (json: Marker) => L.Layer,
        ellipse: (json: Marker) => L.Layer,
        icon: (json: Marker) => L.Layer,
        polygon: (json: Marker) => L.Layer,
        polyline: (json: Marker) => L.Layer,
        rectangle: (json: Marker) => L.Layer
    } = {
        "circle": (json: Marker) => L.circle(Util.toLatLng(json.point), {
            ...json.options,
            radius: Util.pixelsToMeters((json.options as L.CircleOptions)?.radius ?? 0)
        }),
        "ellipse": (json: Marker) => L.ellipse(Util.toLatLng(json.point), json.options as L.EllipseOptions),
        "icon": (json: Marker) => L.marker(Util.toLatLng(json.point), {
            ...json.options,
            "icon": L.icon({...json.options as L.IconOptions})
        }),
        "polygon": (json: Marker) => L.polygon([[0, 0], [0, 0]], json.options as L.PolylineOptions),
        "polyline": (json: Marker) => L.polyline([[0, 0], [0, 0]], json.options as L.PolylineOptions),
        "rectangle": (json: Marker) => L.rectangle([[0, 0], [0, 0]], json.options as L.PolylineOptions)
    }

    private readonly _livemap: LiveMap;
    private readonly _url: string;

    private _label?: string;
    private _interval?: number;

    constructor(livemap: LiveMap, url: string) {
        super([]);

        this._livemap = livemap;
        this._url = url;

        // initial update to get interval and label
        this.update();
    }

    get label(): string {
        return this._label ?? "";
    }

    public tick(count: number): void {
        if (count % (this._interval ?? 0) == 0) {
            this.update();
        }
    }

    private initial(layer: Layer): void {
        this._label = layer.label;
        this._livemap.markersControl?.addOverlay(this);

        if (!layer.hidden) {
            // only add to the map if we
            // are not hiding it by default
            this._livemap.addLayer(this);
            //this.addTo(this._livemap);
        }
    }

    private update(): void {
        Util.fetchJson(this._url).then((layer: Layer): void => {
            try {
                // update refresh interval
                this._interval = layer.interval;

                // update the layer options by merging them in with the defaults
                this.options = {
                    ...layer.options
                };

                if (!this._label) {
                    // this is the first tick
                    this.initial(layer);
                }

                // refresh markers
                this.refresh(layer);
            } catch (e) {
                console.error("ERRoR", e);
            }
        });
    }

    private refresh(layer: Layer): void {
        const queue: L.Layer[] = [];

        // get all markers from json
        layer.markers.forEach((marker: Marker): void => {
            try {
                const type = MarkersLayer._types[marker.type as keyof typeof MarkersLayer._types];
                const toAdd: L.Layer | undefined = type ? type(marker) : undefined;

                if (!toAdd) {
                    return;
                }

                if (marker.popup) {
                    toAdd.bindPopup(marker.popup.content as string, marker.popup);
                }

                if (marker.tooltip) {
                    toAdd.bindTooltip(marker.tooltip.content as string, marker.tooltip);
                }

                queue.push(toAdd);
            } catch (e) {
                console.error(e);
            }
        });

        // todo - keep track of individual markers to prevent flashing on update

        // remove all old markers
        this.clearLayers();

        // add all new markers
        queue.forEach((l: L.Layer): void => {
            this.addLayer(l);
        });
    }
}
