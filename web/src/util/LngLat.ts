import * as L from "leaflet";

export class LngLat extends L.LatLng {
    constructor(longitude: number, latitude: number, altitude?: number) {
        super(latitude, longitude, altitude);
    }
}
