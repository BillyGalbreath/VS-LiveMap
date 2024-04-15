import * as L from 'leaflet';
import {Marker, MarkerJson} from "./Marker";
import {MarkersLayer} from "../MarkersLayer";
import {Location} from "../../data/Location";

export class Icon extends Marker {
    private static svgHtml: string = '<svg preserveAspectRatio="none" width="100%" height="100%"><use href="{0}"></use></svg>';

    constructor(layer: MarkersLayer, json: MarkerJson) {
        const iconOptions: L.IconOptions = json.options as L.IconOptions;
        super(layer, json, L.marker(Location.of(json.point).toLatLng(), {
            ...json.options,
            'icon': iconOptions?.iconUrl?.startsWith('#svg-') ?
                L.divIcon({
                    ...iconOptions,
                    className: '',
                    html: Icon.svgHtml.replace('{0}', iconOptions.iconUrl)
                }) :
                L.icon(iconOptions)
        }));
    }

    public override update(json: MarkerJson): void {
        super.update(json);
        const icon: L.Marker = this._marker as L.Marker;
        icon.setLatLng(Location.of(json.point).toLatLng());
        Object.assign(icon.options, json.options);
        Object.assign(icon.options.icon!.options, json.options as L.IconOptions);
        //(this._marker as L.Marker).getElement()?.setAttribute('aria-label', icon.options?.title ?? icon.options.alt ?? '');
    }
}
