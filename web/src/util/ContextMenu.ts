import * as L from 'leaflet';
import {Util} from "./Util";
import {LiveMap} from "../LiveMap";
import '../svg/star2.svg';

export class ContextMenu {
    private readonly _livemap: LiveMap;
    private readonly _menu: HTMLElement[] = [];

    private _cur: boolean = false;

    constructor(livemap: LiveMap) {
        this._livemap = livemap;

        // get the default menu and clone it
        this._menu[0] = document.querySelector(".contextmenu")!;
        this._menu[1] = this._menu[0].cloneNode(true) as HTMLElement;
        document.body.appendChild(this._menu[1]);

        // prevent right clicking the menus from opening the default menu
        this._menu[0].addEventListener('contextmenu', (e: MouseEvent): void => {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
        });

        this._menu[1].addEventListener('contextmenu', (e: MouseEvent): void => {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
        });
    }

    public open(e: L.LeafletMouseEvent): void {
        // switch to next menu
        const menu: HTMLElement = this._menu[+(this._cur = !this._cur)];

        // show the menu
        menu.classList.add('show');
        menu.style.top = `${Math.min(window.innerHeight - menu.offsetHeight - 25, e.containerPoint.y)}px`;
        menu.style.left = `${Math.min(window.innerWidth - menu.offsetWidth - 25, e.containerPoint.x)}px`;

        // update menu contents
        const point: [number, number] = this._livemap.coordsControl.getCoordinates();
        const icons: NodeListOf<Element> = menu.querySelectorAll(".icon");
        icons.item(1).replaceChildren(Util.createSVGIcon('link'));
        icons.item(3).replaceChildren(Util.createSVGIcon('star2'));
        menu.querySelectorAll(".text").item(0).textContent = `Location: [ ${point[0]}, ${point[1]} ]`;

        // stop the event from bubbling up the stack
        e.originalEvent.preventDefault();
        e.originalEvent.stopPropagation();
        e.originalEvent.stopImmediatePropagation();
    }

    public close(): void {
        this._menu[+this._cur].classList.remove('show');
    }
}
