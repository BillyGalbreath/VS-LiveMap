export class Lang {
    private readonly _title: string;
    private readonly _pinned: string;
    private readonly _unpinned: string;
    private readonly _players: string;
    private readonly _renderers: string;
    private readonly _copy: string;
    private readonly _copyAlt: string;
    private readonly _paste: string;
    private readonly _pasteAlt: string;
    private readonly _share: string;
    private readonly _shareAlt: string;
    private readonly _center: string;
    private readonly _centerAlt: string;
    private readonly _notifCopy: string;
    private readonly _notifCopyFailed: string;
    private readonly _notifPaste: string;
    private readonly _notifPasteFailed: string;
    private readonly _notifPasteInvalid: string;
    private readonly _notifShare: string;
    private readonly _notifShareFailed: string;
    private readonly _notifCenter: string;

    constructor(lang?: Lang) {
        this._title = lang?.title ?? 'Vintage Story LiveMap';
        this._pinned = lang?.pinned ?? 'Pinned';
        this._unpinned = lang?.unpinned ?? 'Unpinned';
        this._players = lang?.players ?? 'Players ({cur}/{max})';
        this._renderers = lang?.renderers ?? 'Map Types';
        this._copy = lang?.copy ?? 'Copy';
        this._copyAlt = lang?.copyAlt ?? 'Copy this location to the clipboard';
        this._paste = lang?.paste ?? 'Paste';
        this._pasteAlt = lang?.pasteAlt ?? 'Go to a point from the clipboard';
        this._share = lang?.share ?? 'Share';
        this._shareAlt = lang?.shareAlt ?? 'Copy a sharable url to the clipboard';
        this._center = lang?.center ?? 'Share';
        this._centerAlt = lang?.centerAlt ?? 'Center the map on this point';
        this._notifCopy = lang?.notifCopy ?? 'Copied location to clipboard';
        this._notifCopyFailed = lang?.notifCopyFailed ?? 'Could not copy location';
        this._notifPaste = lang?.notifPaste ?? 'Centered on location from clipboard';
        this._notifPasteFailed = lang?.notifPasteFailed ?? 'Could not paste location';
        this._notifPasteInvalid = lang?.notifPasteInvalid ?? 'Not a valid location';
        this._notifShare = lang?.notifShare ?? 'Copied shareable url to clipboard';
        this._notifShareFailed = lang?.notifShareFailed ?? 'Could not copy shareable url';
        this._notifCenter = lang?.notifCenter ?? 'Centered on location';
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

    get copy(): string {
        return this._copy;
    };

    get copyAlt(): string {
        return this._copyAlt;
    };

    get paste(): string {
        return this._paste;
    };

    get pasteAlt(): string {
        return this._pasteAlt;
    };

    get share(): string {
        return this._share;
    };

    get shareAlt(): string {
        return this._shareAlt;
    };

    get center(): string {
        return this._center;
    };

    get centerAlt(): string {
        return this._centerAlt;
    };

    get notifCopy(): string {
        return this._notifCopy;
    };

    get notifCopyFailed(): string {
        return this._notifCopyFailed;
    };

    get notifPaste(): string {
        return this._notifPaste;
    };

    get notifPasteFailed(): string {
        return this._notifPasteFailed;
    };

    get notifPasteInvalid(): string {
        return this._notifPasteInvalid;
    };

    get notifShare(): string {
        return this._notifShare;
    };

    get notifShareFailed(): string {
        return this._notifShareFailed;
    };

    get notifCenter(): string {
        return this._notifCenter;
    };
}
