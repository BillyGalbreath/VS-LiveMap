/**
 * MIT License
 *
 * Copyright (c) 2024 William Blake Galbreath
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

export type Contents = [
    text: string,
    icon: string,
    key: string,
    action: () => null
];

export abstract class ContextMenu {
    private readonly _currentMenu: [HTMLElement, HTMLElement];

    private _cur: boolean = false;

    protected constructor(contents: Contents[]) {
        // create dual menus to swap between
        this._currentMenu = [
            this.createMenu(contents),
            this.createMenu(contents)
        ];

        window.onblur = document.onblur = (): void => this.close();
        window.onkeydown = (e: KeyboardEvent) => this.onKey(e);
        window.oncontextmenu = (e: MouseEvent) => this.open(e);
    }

    get currentMenu(): HTMLElement | null {
        // get the current active menu
        return document.querySelector('.contextmenu.show');
    }

    protected abstract onOpen(menu: HTMLElement): void;

    protected abstract onClose(menu: HTMLElement): void;

    protected abstract onKey(e: KeyboardEvent): void;

    protected open(e: MouseEvent): void {
        // get the next menu
        const menu: HTMLElement = this._currentMenu[+(this._cur = !this._cur)];

        // show the menu at mouse position, while keeping it inside the viewable area
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.x)}px`;

        // stop the event from bubbling up the stack
        this.stopPropagation(e);

        // allow implementation to run stuff
        this.onOpen(menu);
    }

    protected close(): void {
        const menu: HTMLElement | null = this.currentMenu;
        if (menu) {
            // hide the current menu
            menu.classList.remove('show');

            // allow implementation to run stuff
            this.onClose(menu);
        }
    }

    private createMenu(contents: Contents[]): HTMLElement {
        // create the menu
        const menu: HTMLElement = document.createElement('div');
        menu.classList.add('contextmenu');

        // add the rows
        contents.forEach((content: Contents): void => menu.append(this.createRow(content)));

        // prevent menu from opening real context menu
        menu.addEventListener('contextmenu', (e: Event): void => this.stopPropagation(e));

        // add menu to body and return the menu
        return document.body.appendChild(menu);
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
}
