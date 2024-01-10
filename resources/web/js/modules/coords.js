import {LM} from '../livemap.js';

export class Coords {
    constructor(enabled) {
        // coords control box extension
        const CoordsBox = L.Control.extend({
            _container: null,
            options: {
                position: 'bottomleft'
            },
            onAdd: function () {
                const coords = L.DomUtil.create('div', 'leaflet-control-layers coordinates');
                this._div = coords;
                return coords;
            },
            update: function (point) {
                this.x = point == null ? "---" : Math.round(point.x);
                this.z = point == null ? "---" : Math.round(point.y);
                this._div.innerHTML = this.x + "," + this.z;
            }
        });
        // create the coords control box
        this.coords = new CoordsBox();
        LM.map.addControl(this.coords).addEventListener('mousemove', (event) => {
            this.coords.update(LM.toPoint(event.latlng));
        });
        if (!enabled) {
            this.coords._div.style.display = "none";
        }
        this.coords.update();
    }
}
