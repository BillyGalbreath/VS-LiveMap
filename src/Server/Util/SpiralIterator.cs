using System;
using System.Diagnostics.CodeAnalysis;
using Vintagestory.API.MathTools;

namespace LiveMap.Server.Util;

public class SpiralIterator {
    private int _x;
    private int _z;

    private long _total;
    private long _current;
    private long _axis;

    private Direction _direction;

    public SpiralIterator(int x, int z) {
        _x = x;
        _z = z;
    }

    public bool HasNext() {
        return _current < _total;
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public Vec2i Next() {
        if (!HasNext()) {
            throw new ArgumentOutOfRangeException();
        }

        // get current coordinate
        Vec2i point = new(_x, _z);

        switch (_direction) {
            case Direction.East:
                ++_x;
                break;
            case Direction.South:
                ++_z;
                break;
            case Direction.West:
                --_x;
                break;
            case Direction.North:
                --_z;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // calculate where we are in the spiral
        ++_current;
        if (_current > _total) {
            _direction = _direction.Next();
            _current = 0;
            ++_axis;
            if (_axis > 1) {
                _axis = 0;
                _total++;
            }
        }

        return point;
    }
}

public enum Direction {
    East,
    South,
    West,
    North
}

public static class DirectionExtensions {
    public static Direction Next(this Direction direction) {
        return (Direction)(((int)direction + 1) & 3);
    }
}
