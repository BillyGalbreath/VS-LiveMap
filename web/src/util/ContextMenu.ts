import {LiveMap} from "../LiveMap";
import {Util} from "./Util";

export interface Contents {
    text: string;
    icon: string;
    key: string;
    action: (e?: Event) => null;
}

export class ContextMenu {
    private readonly _livemap: LiveMap;
    private readonly _pointRegex: RegExp = /\[? ?(-?\d+) ?,? ?(-?\d+) ?]?/;
    private readonly _contents: Contents[];

    public constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._contents = [
            {text: '[ 0,0 ]'},
            {}
        ] as Contents[];

        if (navigator.clipboard) {
            // some browsers don't support clipboard api :(
            this._contents = this._contents.concat([
                {text: 'Copy', icon: 'copy', key: 'Ctrl+C', action: () => this.copy()},
                {text: 'Paste', icon: 'paste', key: 'Ctrl+V', action: () => this.paste()},
                {}
            ] as Contents[]);
        }

        this._contents = this._contents.concat([
            {text: 'Share', icon: 'link', key: 'Ctrl+S', action: () => this.share()},
            {text: 'Bookmark', icon: 'star2', key: 'Ctrl+B', action: (e: KeyboardEvent) => this.bookmark(e)},
            {},
            {text: 'Center', icon: 'center', key: 'F10', action: () => this.center()}
        ] as Contents[]);

        // create dual menus to swap between
        this.createMenu();
        this.createMenu();

        // setup map's listeners
        window.onblur = document.onblur = (): void => this.close();
        window.oncontextmenu = (e: MouseEvent) => this.open(e);
        window.onkeydown = (e: KeyboardEvent) => this.keydown(e);

        this._livemap.on('load unload resize viewreset move movestart moveend zoom zoomstart' +
            'zoomend zoomlevelschange popupopen popupclose tooltipopen tooltipclose click dblclick' +
            'mousedown mouseup preclick', (): void => this.close());
    }

    get shownMenu(): HTMLElement | null {
        // get the current active menu
        return document.querySelector('.wrapper.show');
    }

    get hiddenMenu(): HTMLElement | null {
        // get the first inactive menu
        return document.querySelector('.wrapper:not(.show)');
    }

    private close(): void {
        // hide the current menu
        const menu: HTMLElement | null = this.shownMenu;
        if (menu) {
            // delay this so our query selector works correctly elsewhere
            // closing too quickly will just make open use the same menu
            // again which effectively skips the open/close animations
            setTimeout((): void => {
                menu.classList.remove('show');
            }, 50);
        }
    }

    private open(e: MouseEvent): void {
        // close current menu (mobile needs this)
        this.close();

        // get the next menu
        const menu: HTMLElement = this.hiddenMenu!;

        // show the menu at mouse position, while keeping it inside the viewable area
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.x)}px`;

        // stop the event from bubbling up the stack
        this.stopPropagation(e);

        // update coordinates in first row
        menu.querySelector('div:first-child p:nth-child(2)')!.innerHTML = this.getPoint();
    }

    private keydown(e: KeyboardEvent): void {
        // close menu when escape is pressed
        if (e.key === 'Escape') {
            this.close();
            return;
        }

        // get current key combo
        const combo: string = this.combo(e);

        // find and run any actions for this key combo
        this._contents.forEach((contents: Contents): void => {
            if (contents.key == combo && contents.action) {
                contents.action(e);
            }
        });
    }

    private createMenu(): void {
        // create the menu
        const wrapper: HTMLElement = document.createElement('div');
        const div: HTMLElement = document.createElement('div');
        const menu: HTMLElement = document.createElement('div');

        wrapper.appendChild(div);
        div.appendChild(menu);

        wrapper.classList.add('wrapper');
        menu.classList.add('contextmenu');

        // add the rows
        this._contents.forEach((contents: Contents): void => menu.append(this.createRow(contents)));

        // prevent menu from opening real context menu
        menu.addEventListener('contextmenu', (e: Event): void => this.stopPropagation(e));

        // add menu to body
        document.body.appendChild(wrapper);
    }

    private createRow(contents: Contents): HTMLElement {
        if (!contents || !contents.text) {
            // no contents or text, make a divider
            return document.createElement('hr');
        }

        // init contents
        const iconElem: Element = document.createElement('p');
        const textElem: Element = document.createElement('p');
        const keyElem: Element = document.createElement('p');

        // populate contents
        if (contents.icon) {
            const template: HTMLTemplateElement = document.createElement('template');
            template.innerHTML = `<svg><use href='#svg-${contents.icon}'></use></svg>`;
            iconElem.appendChild(template.content);
        }
        textElem.innerHTML = contents.text ?? '';
        keyElem.innerHTML = contents.key ?? '';

        // create row
        const row: HTMLElement = document.createElement('div');
        row.appendChild(iconElem);
        row.appendChild(textElem);
        row.appendChild(keyElem);

        // attach click action
        if (contents.action) {
            row.onclick = (e: MouseEvent) => contents.action(e);
        }

        return row;
    }

    private stopPropagation(e: Event): void {
        // stop the event in the current element
        // and prevent it from bubbling up the stack
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
    }

    private combo(e: KeyboardEvent): string {
        let combo: string = '';
        if (e.ctrlKey) {
            combo += (combo ? '+' : '') + 'Ctrl'
        }
        if (e.shiftKey) {
            combo += (combo ? '+' : '') + 'Shift'
        }
        if (e.altKey) {
            combo += (combo ? '+' : '') + 'Alt'
        }
        if (e.key !== 'Control' && e.key !== 'Shift' && e.key !== 'Alt') {
            combo += (combo ? '+' : '') + e.key.charAt(0).toUpperCase() + e.key.substring(1);
        }
        return combo;
    }

    private getPoint(): string {
        const point: [number, number] = this._livemap.coordsControl.getCoordinates();
        return `[ ${point[0]}, ${point[1]} ]`;
    }

    public copy(): void {
        let text: string | undefined = this.shownMenu?.querySelector('p:nth-child(2)')?.innerHTML;
        //Util.copyToClipboard(text = text ?? this.getPoint())
        navigator.clipboard.writeText(text = text ?? this.getPoint())
            .then((): void => {
                console.log(`copied point '${text}' to clipboard`)
                this.close();
            })
            .catch((e): void => {
                console.log(e);
            });
    }

    public paste(): void {
        navigator.clipboard.readText().then((text: string): void => {
            const match: RegExpExecArray | null = this._pointRegex.exec(text);
            console.log(`pasted text '${text}' (is point: ${!!match})`);
            if (!match) {
                return;
            }
            const x: number = parseInt(match[1]);
            const y: number = parseInt(match[2]);
            console.log(`x: ${x}`, `y: ${y}`);
            this._livemap.setView(Util.toLatLng([
                x + this._livemap.settings.spawn.x,
                y + this._livemap.settings.spawn.y
            ]));
        });
    }

    public share(): void {
        //
    }

    public bookmark(e: Event): void {
        //

        if (e instanceof KeyboardEvent) {
            this.stopPropagation(e);
        }
    }

    public center(): void {
        //
    }
}
