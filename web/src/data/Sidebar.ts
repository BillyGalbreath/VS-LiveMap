export class Sidebar {
    private readonly _pinned: string;

    constructor(sidebar?: Sidebar) {
        this._pinned = sidebar?.pinned ?? 'unpinned';
    }

    get pinned(): string {
        return this._pinned;
    }
}
