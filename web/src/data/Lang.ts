export class Lang {
    private readonly _title: string;
    private readonly _pinned: string;
    private readonly _unpinned: string;
    private readonly _players: string;
    private readonly _renderers: string;

    constructor(lang?: Lang) {
        this._title = lang?.title ?? 'Vintage Story LiveMap';
        this._pinned = lang?.pinned ?? 'Pinned';
        this._unpinned = lang?.unpinned ?? 'Unpinned';
        this._players = lang?.players ?? 'Players ({cur}/{max})';
        this._renderers = lang?.renderers ?? 'Map Types';
    }

    get title(): string {
        return this._title;
    }

    get pinned(): string {
        return this._pinned;
    }

    get unpinned(): string {
        return this._unpinned;
    }

    get players(): string {
        return this._players;
    }

    get renderers(): string {
        return this._renderers;
    }
}
