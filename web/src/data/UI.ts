import {Sidebar} from './Sidebar';

export class UI {
    private readonly _sidebar: Sidebar;

    constructor(ui?: UI) {
        this._sidebar = ui?.sidebar ? new Sidebar(ui.sidebar) : new Sidebar();
    }

    get sidebar(): Sidebar {
        return this._sidebar;
    }
}
