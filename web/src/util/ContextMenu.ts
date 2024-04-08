import {LiveMap} from "../LiveMap";
import {Util} from "./Util";

export type Contents = [
    text: string,
    icon: string,
    key: string,
    action: () => null
];

export class ContextMenu {
    private readonly _livemap: LiveMap;
    private readonly _pointRegex: RegExp = /\[? ?(-?\d+) ?,? ?(-?\d+) ?]?/;
    private readonly _menus: [HTMLElement, HTMLElement];
    private readonly _contents: Contents[];

    private _cur: boolean = false;

    public constructor(livemap: LiveMap) {
        this._livemap = livemap;

        this._contents = [
            ['[ 0,0 ]'],
            [],
            ['Copy', 'copy', 'Ctrl+C', () => this.copyPoint()],
            ['Paste', 'paste', 'Ctrl+V', () => this.pastePoint()],
            [],
            ['Share', 'link', 'Ctrl+S'],
            ['Bookmark', 'star2', 'Ctrl+B'],
            [],
            ['Center', 'center', 'F10']
        ] as Contents[];

        // create dual menus to swap between
        this._menus = [
            this.createMenu(),
            this.createMenu()
        ];

        // setup map's listeners
        window.onblur = document.onblur = (): void => this.close();
        window.onkeydown = (e: KeyboardEvent) => this.keydown(e);
        window.oncontextmenu = (e: MouseEvent) => this.open(e);
        this._livemap.on('load unload resize viewreset move movestart moveend zoom zoomstart' +
            'zoomend zoomlevelschange popupopen popupclose tooltipopen tooltipclose click dblclick' +
            'mousedown mouseup preclick', (): void => this.close());
    }

    get currentMenu(): HTMLElement | null {
        // get the current active menu
        return document.querySelector('.wrapper.show');
    }

    private open(e: MouseEvent): void {
        //close();

        // get the next menu
        const menu: HTMLElement = this._menus[+(this._cur = !this._cur)];

        // show the menu at mouse position, while keeping it inside the viewable area
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.x)}px`;

        // stop the event from bubbling up the stack
        this.stopPropagation(e);

        // update coordinates in first row
        menu.querySelector('div:first-child p:nth-child(2)')!.innerHTML = this.getPoint();
    }

    private close(): void {
        // hide the current menu
        this.currentMenu?.classList.remove('show');
    }

    private keydown(e: KeyboardEvent): void {
        class Key {
            public combo: string = '';

            public add(key: string): string {
                return this.combo += (this.combo ? '+' : '') + key;
            }
        }

        let key: Key = new Key();
        if (e.ctrlKey) {
            key.add('Ctrl');
        }
        if (e.shiftKey) {
            key.add('Shift');
        }
        if (e.altKey) {
            key.add('Alt');
        }
        if (e.key !== 'Control' && e.key !== 'Shift' && e.key !== 'Alt') {
            key.add(e.key.charAt(0).toUpperCase() + e.key.substring(1));
        }

        this._contents.forEach((contents: Contents): void => {
            if (contents[2] == key.combo && contents[3]) {
                contents[3]();
            }
        });

        if (e.key === 'Escape') {
            this.close();
        }
        if (key.combo === 'Ctrl+B') {
            this.stopPropagation(e);
        }
    }

    private createMenu(): HTMLElement {
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

        // add menu to body and return the menu
        return document.body.appendChild(wrapper);
    }

    private createRow(contents: Contents): HTMLElement {
        if (!contents || !contents[0]) {
            // no contents or text, make a divider
            return document.createElement('hr');
        }

        // init contents
        const iconElem: Element = document.createElement('p');
        const textElem: Element = document.createElement('p');
        const keyElem: Element = document.createElement('p');

        // populate contents
        if (contents[1]) {
            const template: HTMLTemplateElement = document.createElement('template');
            template.innerHTML = `<svg><use href='#svg-${contents[1]}'></use></svg>`;
            iconElem.appendChild(template.content);
        }
        textElem.innerHTML = contents[0] ?? '';
        keyElem.innerHTML = contents[2] ?? '';

        // create row
        const row: HTMLElement = document.createElement('div');
        row.appendChild(iconElem);
        row.appendChild(textElem);
        row.appendChild(keyElem);

        // attach click action
        if (contents[3]) {
            row.onclick = () => contents[3]();
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

    private getPoint(): string {
        const point: [number, number] = this._livemap.coordsControl.getCoordinates();
        return `[ ${point[0]}, ${point[1]} ]`;
    }

    public copyPoint(): void {
        let text: string | undefined = this.currentMenu?.querySelector('p:nth-child(2)')?.innerHTML;
        navigator.clipboard.writeText(text = text ?? this.getPoint()).then((): void => {
            console.log(`copied point '${text}' to clipboard`)
            this.close();
        });
    }

    public pastePoint(): void {
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
}
