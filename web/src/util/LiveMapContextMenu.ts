import {LiveMap} from "../LiveMap";
import {Contents, ContextMenu} from "../lib/ContextMenu";

export class LiveMapContextMenu extends ContextMenu {
    private readonly _livemap: LiveMap;

    constructor(livemap: LiveMap) {
        super([
            ['[ 0,0 ]'],
            [],
            ['Copy', 'copy', 'Ctrl+C', () => this.copyPoint()],
            ['Paste', 'paste', 'Ctrl+V', () => this.pastePoint()],
            [],
            ['Share', 'link', 'Ctrl+S'],
            ['Bookmark', 'star2', 'Ctrl+B'],
            [],
            ['Center', 'center', 'F10']
        ] as Contents[]);
        this._livemap = livemap;

        // setup map's listeners
        this._livemap.on('load unload resize viewreset move movestart moveend zoom zoomstart' +
            'zoomend zoomlevelschange popupopen popupclose tooltipopen tooltipclose click dblclick' +
            'mousedown mouseup preclick', (): void => this.close());
    }

    protected override onKey(e: KeyboardEvent): void {
        if (e.key === 'Escape') {
            this.close();
        }
    }

    protected override onClose(menu: HTMLElement): void {
    }

    protected override onOpen(menu: HTMLElement): void {
        const point: [number, number] = this._livemap.coordsControl.getCoordinates();
        menu.querySelector('div:first-child p:nth-child(2)')!.innerHTML = `[ ${point[0]}, ${point[1]} ]`;
    }

    public copyPoint(): void {
        const text: string | undefined = this.currentMenu?.querySelector('p:nth-child(2)')?.innerHTML;
        if (text) {
            navigator.clipboard.writeText(text).then((): void => {
                console.log(`copied point '${text}' to clipboard`)
                this.close();
            });
        }
    }

    public pastePoint(): void {
        navigator.clipboard.readText().then((text: string): void => {
            console.log('text', text);
        });
    }
}
