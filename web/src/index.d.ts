import {LiveMap} from "./LiveMap";

declare global {
    interface Window {
        livemap: LiveMap
    }
}
