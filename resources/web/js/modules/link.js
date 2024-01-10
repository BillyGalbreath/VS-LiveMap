import {LM} from '../livemap.js';

export class Link {
    constructor(enabled) {
        const Link = L.Control.extend({
            _container: null,
            options: {
                position: 'bottomleft'
            },
            onAdd: function () {
                const link = L.DomUtil.create('div', 'leaflet-control-layers link');
                this._link = link;
                return link;
            },
            update: function () {
                this._link.innerHTML = `<a href='${LM.getUrlFromView()}'><img src='images/clear.png' alt=''/></a>`;
            }
        });
        this.link = new Link();
        LM.map.addControl(this.link)
            .addEventListener('move', () => this.update())
            .addEventListener('zoom', () => this.update());
        if (!enabled) {
            this.link._link.style.display = "none";
        }
        this.link.update();
    }

    update() {
        this.link.update();
    }
}
