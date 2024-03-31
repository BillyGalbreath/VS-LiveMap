export class Point {
    private readonly _x: number;
    private readonly _z: number;

    constructor(x?: number, z?: number) {
        this._x = x ?? 0;
        this._z = z ?? 0;
    }

    get x(): number {
        return this._x;
    }

    get z(): number {
        return this._z;
    }

    public add(num: number): Point {
        return new Point(this.x + num, this.z + num);
    }

    public sub(num: number): Point {
        return new Point(this.x - num, this.z - num);
    }

    public div(num: number): Point {
        return new Point(this.x / num, this.z / num);
    }

    public mul(num: number): Point {
        return new Point(this.x * num, this.z * num);
    }
}
