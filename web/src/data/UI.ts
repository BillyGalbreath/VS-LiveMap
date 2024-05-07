import {Sidebar} from './Sidebar';

export class UI {
    private readonly _sidebar: Sidebar;

    constructor(ui?: UI) {
        this._sidebar = new Sidebar(ui?.sidebar);
    }

    get sidebar(): Sidebar {
        return this._sidebar;
    }
}
