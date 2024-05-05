import {Point} from './Point';
import {Value} from './Value';

export class Player {
    private readonly _id: string;
    private readonly _name: string;
    private readonly _avatar: string;
    private readonly _role: string;
    private readonly _pos: Point;
    private readonly _yaw: number;
    private readonly _health: Value;
    private readonly _satiety: Value;

    constructor(player?: Player) {
        this._id = player?.id ?? '';
        this._name = player?.name ?? '';
        this._avatar = player?.avatar ?? '';
        this._role = player?.role ?? '';
        this._pos = player?.pos ?? new Point(0, 0);
        this._yaw = player?.yaw ?? 0;
        this._health = player?.health ?? new Value();
        this._satiety = player?.satiety ?? new Value();
    }

    get id(): string {
        return this._id;
    }

    get name(): string {
        return this._name;
    }

    get avatar(): string {
        return this._avatar;
    }

    get role(): string {
        return this._role;
    }

    get pos(): Point {
        return this._pos;
    }

    get yaw(): number {
        return this._yaw;
    }

    get health(): Value {
        return this._health;
    }

    get satiety(): Value {
        return this._satiety;
    }
}
