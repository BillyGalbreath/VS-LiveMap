import * as L from 'leaflet';
import {LiveMap} from "../LiveMap";

export class Notifications {
    private readonly _dom: HTMLElement;

    constructor() {
        this._dom = L.DomUtil.create('div', 'notifications');
        document.body.appendChild(this._dom);
    }

    public create(type: ('info' | 'success' | 'warning' | 'danger'), text: string): void {
        const div: HTMLElement = L.DomUtil.create('div', `${type}`);
        div.appendChild(LiveMap.createSVGIcon(type));

        const p: HTMLElement = div.appendChild(L.DomUtil.create('p'));
        p.innerText = text;

        this._dom.appendChild(div);

        setTimeout((): void => {
            div.classList.add('show');
            const handler = (): void => {
                div.removeEventListener('transitionend', handler);
                setTimeout((): void => {
                    div.classList.remove('show');
                    div.addEventListener('transitionend', (): void => {
                        try {
                            this._dom.removeChild(div);
                        } catch (e) {
                        }
                    });
                }, 2500);
            };
            div.addEventListener('transitionend', handler);
        }, 100);
    }

    public info(text: string): void {
        this.create('info', text);
    }

    public success(text: string): void {
        this.create('success', text);
    }

    public warning(text: string): void {
        this.create('warning', text);
    }

    public danger(text: string): void {
        this.create('danger', text);
    }
}
