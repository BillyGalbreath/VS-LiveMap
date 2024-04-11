import {LiveMap} from "../LiveMap";
import {Location} from "../data/Location";

export interface Contents {
    text: string;
    icon: string;
    key: string;
    action: (e?: Event) => null;
}

export class ContextMenu {
    private readonly _livemap: LiveMap;
    private readonly _locRegex: RegExp = /\[? ?(-?\d+) ?,? ?(-?\d+) ?]?/;

    private readonly _menu: [HTMLElement, HTMLElement];
    private readonly _contents: Contents[];

    private _loc?: Location;
    private _cur: boolean = false;

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
                {},
                {text: 'Share', icon: 'link', key: 'Ctrl+S', action: () => this.share(this._loc)},
                {}
            ] as Contents[]);
        }

        this._contents = this._contents.concat([
            {text: 'Center', icon: 'center', key: 'F10', action: () => this.center()}
        ] as Contents[]);

        // create dual menus to swap between
        this._menu = [
            this.createMenu(),
            this.createMenu()
        ];

        // setup map's listeners
        window.onblur = document.onblur = (): void => this.close();
        window.oncontextmenu = (e: MouseEvent) => this.open(e);
        window.onkeydown = (e: KeyboardEvent) => this.keydown(e);

        this._livemap.on('load unload resize viewreset move movestart moveend zoom zoomstart zoomend' +
            ' zoomlevelschange click dblclick mousedown mouseup preclick', (): void => this.close());
    }

    get nextMenu(): HTMLElement {
        // get the current active menu
        return this._menu[+(this._cur = !this._cur)];
    }

    get currentMenu(): HTMLElement {
        // get the current menu
        return this._menu[+this._cur];
    }

    private close(): void {
        // hide the current menu
        this.currentMenu.classList.remove('show');

        this._loc = undefined;
    }

    private open(e: MouseEvent): void {
        // close current menu (mobile needs this)
        this.close();

        // get the next menu
        const menu: HTMLElement = this.nextMenu;

        // show the menu at mouse position, while keeping it inside the viewable area
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.x)}px`;

        // stop the event from bubbling up the stack
        this.stopPropagation(e);

        // update coordinates in first row
        menu.querySelector('div:first-child p:nth-child(2)')!.innerHTML = this.getLocation();

        this._loc = this._livemap.coordsControl.getLocation();
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
                this.stopPropagation(e);
            }
        });
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

        // add menu to body
        document.body.appendChild(wrapper);

        return wrapper;
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
            iconElem.appendChild(this._livemap.createSVGIcon(contents.icon));
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

    private getLocation(loc?: Location): string {
        loc ??= this._livemap.coordsControl.getLocation();
        return `[ ${loc.x}, ${loc.z} ]`;
    }

    public copy(): void {
        //const text: string | undefined = this.currentMenu.querySelector('p:nth-child(2)')?.innerHTML;
        navigator.clipboard.writeText(this.getLocation())
            .then((): void => {
                window.livemap.notifications.success('Copied location to clipboard');
            })
            .catch((e): void => {
                console.error('Could not copy location\n', e);
                window.livemap.notifications.danger('Could not copy location');
            })
            .finally((): void => {
                this.close();
            });
    }

    public paste(): void {
        navigator.clipboard.readText()
            .then((text: string): void => {
                const match: RegExpExecArray | null = this._locRegex.exec(text);
                if (!match) {
                    this._livemap.notifications.warning('Not a valid location');
                    return;
                }
                this._livemap.notifications.info('Centered on location from clipboard');
                this._livemap.centerOn(Location.of(parseInt(match[1]), parseInt(match[2])));
            })
            .catch((e): void => {
                console.error('Could not paste location\n', e);
                this._livemap.notifications.danger('Could not paste location');
            })
            .finally((): void => {
                this.close();
            });
    }

    public share(loc?: Location): void {
        loc ??= this._livemap.coordsControl.getLocation();
        const text: string = `${window.location.origin}${window.location.pathname}?x=${loc.x}&z=${loc.z}&zoom=${this._livemap.currentZoom()}`;
        navigator.clipboard.writeText(text)
            .then((): void => {
                window.livemap.notifications.success('Copied shareable url to clipboard');
            })
            .catch((e): void => {
                console.error('Could not copy shareable url\n', e);
                window.livemap.notifications.danger('Could not copy shareable url');
            })
            .finally((): void => {
                this.close();
            });
    }

    public center(): void {
        this._livemap.centerOn(this._livemap.coordsControl.getLocation());
        this._livemap.coordsControl.update();
        this._livemap.linkControl.update();
    }
}
