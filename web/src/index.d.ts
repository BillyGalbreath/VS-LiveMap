import {Livemap} from "./livemap";

declare global {
    interface Window {
        livemap: Livemap
    }
}
