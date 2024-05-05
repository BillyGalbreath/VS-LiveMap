export class Sidebar {
    private readonly _pinned: string;

    constructor(sidebar?: Sidebar) {
        this._pinned = sidebar?.pinned ?? 'false';
    }

    get pinned(): string {
        return this._pinned;
    }
}
