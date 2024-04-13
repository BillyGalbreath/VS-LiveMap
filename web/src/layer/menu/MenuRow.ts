import {LiveMap} from "../../LiveMap";

export class MenuRow {
    private readonly _text?: string;
    private readonly _icon?: string;
    private readonly _key?: string;
    private readonly _action?: (e?: Event) => void;

    constructor(text?: string, icon?: string, key?: string, action?: (e?: Event) => void) {
        this._text = text;
        this._icon = icon;
        this._key = key?.toLowerCase();
        this._action = action;
    }

    get text(): string | undefined {
        return this._text;
    }

    get icon(): string | undefined {
        return this._icon;
    }

    get key(): string | undefined {
        return this._key;
    }

    get action(): ((e?: Event) => void) | undefined {
        return this._action;
    }

    public create(): HTMLElement {
        if (!this._text) {
            // no contents or text, make a divider
            return document.createElement('hr');
        }

        // init contents
        const iconElem: Element = document.createElement('p');
        const textElem: Element = document.createElement('p');
        const keyElem: Element = document.createElement('p');

        // populate contents
        if (this._icon) {
            iconElem.appendChild(LiveMap.createSVGIcon(this._icon));
        }
        textElem.innerHTML = this._text ?? '';
        keyElem.innerHTML = this._key ?? '';

        // create row
        const row: HTMLElement = document.createElement('div');
        row.appendChild(iconElem);
        row.appendChild(textElem);
        row.appendChild(keyElem);

        // attach click action
        if (this._action) {
            row.onclick = (e: MouseEvent) => this._action!(e);
        }

        return row;
    }
}
