import * as L from 'leaflet';
import {LiveMap} from "../../LiveMap";
import {MarkersLayer} from "../MarkersLayer";

export interface MarkerJson {
    type: string,
    id: string,
    point: L.PointTuple,
    points: L.PointTuple[],
    options: L.MarkerOptions,
    tooltip: L.TooltipOptions,
    popup: PopupOptions
}

export interface PopupOptions extends L.PopupOptions {
    autoOpen?: boolean | undefined
}

export abstract class Marker {
    protected readonly _id: string;
    protected readonly _marker: L.Layer;
    protected readonly _json: MarkerJson;

    protected constructor(livemap: LiveMap, json: MarkerJson, marker: L.Layer) {
        this._id = '';
        this._json = json;
        this._marker = marker;

        // add popup
        if (this._json.popup) {
            this._marker.bindPopup(this._json.popup.content as string, this._json.popup);
        }

        // add tooltip
        if (this._json.tooltip) {
            this._marker.bindTooltip(this._json.tooltip.content as string, this._json.tooltip);
        }

        // create any panes needed for this marker
        livemap.getOrCreatePane(json.options?.pane);
        livemap.getOrCreatePane(json.popup?.pane);
        livemap.getOrCreatePane(json.tooltip?.pane);
    }

    get id(): string {
        return this._id;
    }

    get(): L.Layer {
        return this._marker;
    }

    public addTo(layer: MarkersLayer): Marker {
        // add to marker layer
        this._marker.addTo(layer);

        // add popup
        if (this._json?.popup?.autoOpen) {
            this._marker.openPopup();
        }

        return this;
    }

    public remove(): void {
        this._marker.remove();
    }

    public abstract update(data: MarkerJson): void;
}
