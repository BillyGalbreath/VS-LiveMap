import {LiveMap} from "../LiveMap";
import {Renderer} from "../data/Renderer";
import * as L from "leaflet";

export class RenderersControl {
    private readonly _livemap: LiveMap;
    private readonly _dom: HTMLElement;

    private _renderers: Renderer[] = [];

    private _rendererType: string = "basic";

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._dom = L.DomUtil.create('ul');

        livemap.settings.renderers.forEach((renderer: Renderer): void => {
            this._renderers.push(new Renderer(renderer))
        });
    }

    get dom(): HTMLElement {
        return this._dom;
    }

    get rendererType(): string {
        return this._rendererType;
    }

    set rendererType(renderer: string | null) {
        this._rendererType = !renderer?.length ? this._renderers[0].id : renderer;
    }
}
