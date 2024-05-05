export class MenuRow {
    private readonly _text?: string;
    private readonly _alt?: string;
    private readonly _icon?: string;
    private readonly _key?: string;
    private readonly _action?: (e?: Event) => void;

    constructor(text?: string, alt?: string, icon?: string, key?: string, action?: (e?: Event) => void) {
        this._text = text;
        this._alt = alt;
        this._icon = icon;
        this._key = key?.toLowerCase();
        this._action = action;
    }

    get text(): string | undefined {
        return this._text;
    }

    get alt(): string | undefined {
        return this._alt;
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
        if (!this.text) {
            // no contents or text, make a divider
            return document.createElement('hr');
        }

        // init contents
        const iconElem: Element = document.createElement('p');
        const textElem: Element = document.createElement('p');
        const keyElem: Element = document.createElement('p');

        // populate contents
        if (this.icon) {
            iconElem.appendChild(window.createSVGIcon(this.icon));
        }
        textElem.innerHTML = this.text ?? '';
        keyElem.innerHTML = this.key ?? '';

        // create row
        const row: HTMLElement = document.createElement('div');
        row.appendChild(iconElem);
        row.appendChild(textElem);
        row.appendChild(keyElem);

        if (this.alt) {
            row.title = this.alt;
        }

        // attach click action
        if (this.action) {
            row.onclick = (e: MouseEvent) => this.action!(e);
        }

        return row;
    }
}
