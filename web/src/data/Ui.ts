export class Ui {
    private readonly _attribution: string;
    private readonly _homepage: string;
    private readonly _title: string;
    private readonly _logo: string;
    private readonly _sidebar: string;

    constructor(json?: Ui) {
        this._attribution = json?.attribution ?? `<a href='https://mods.vintagestory.at/livemap' target='_blank'>Livemap</a> &copy;2024`;
        this._homepage = json?.homepage ?? 'https://mods.vintagestory.at/livemap';
        this._title = json?.title ?? 'Vintage Story LiveMap';
        this._logo = json?.logo ?? 'LiveMap';
        this._sidebar = json?.sidebar ?? 'unpinned';
    }

    get attribution(): string {
        return this._attribution;
    }

    get homepage(): string {
        return this._homepage;
    }

    get title(): string {
        return this._title;
    }

    get logo(): string {
        return this._logo;
    }

    get sidebar(): string {
        return this._sidebar;
    }
}
