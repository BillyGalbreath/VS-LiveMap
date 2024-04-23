using System;
using JetBrains.Annotations;
using livemap.common.api.json;
using Newtonsoft.Json;

namespace livemap.common.api;

/// <summary>
/// Represents a point on the map
/// </summary>
[PublicAPI]
[JsonConverter(typeof(PointJsonConverter))]
public readonly struct Point {
    /// <summary>
    /// The value of the X axis of this point
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The value of the Z axis of this point
    /// </summary>
    public double Z { get; }

    /// <summary>
    /// Create a new point from a coordinate array
    /// </summary>
    /// <param name="arr"></param>
    public Point(double[] arr) : this(arr[0], arr[1]) { }

    /// <summary>
    /// Create a new point from coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public Point(double x, double z) {
        X = x;
        Z = z;
    }

    /// <summary>
    /// Get the axis value of the specified index
    /// </summary>
    /// <param name="index">Index of axis</param>
    /// <remarks>
    /// Index <c>0</c> represents axis <c>X</c><br/>
    /// Index <c>1</c> represents axis <c>Z</c>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public double this[int index] {
        get {
            return index switch {
                0 => X,
                1 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Out of bounds")
            };
        }
    }

    public static implicit operator Point(double[] value) => new(value[0], value[1]);

    public static implicit operator double[](Point value) => new[] { value.X, value.Z };

    /// <summary>
    /// Adds the specified amount to the point's axis values
    /// </summary>
    /// <param name="num">The value to add to the point's axis values</param>
    /// <returns>A new point with the added axis values</returns>
    public Point Add(double num) => this + num;

    /// <summary>
    /// Adds the specified point's axis values to this point's axis values
    /// </summary>
    /// <param name="point">The point to add to this point's axis values</param>
    /// <returns>A new point with the added axis values</returns>
    public Point Add(Point point) => this + point;

    public static Point operator +(Point left, Point right) => new(left.X + right.X, left.Z + right.Z);
    public static Point operator +(Point left, double right) => new(left.X + right, left.Z + right);
    public static Point operator +(double left, Point right) => new(left + right.X, left + right.Z);

    /// <summary>
    /// Subtracts the specified amount to the point's axis values
    /// </summary>
    /// <param name="num">The value to subtract to the point's axis values</param>
    /// <returns>A new point with the subtracted axis values</returns>
    public Point Subtract(double num) => this - num;

    /// <summary>
    /// Subtracts the specified point's axis values to this point's axis values
    /// </summary>
    /// <param name="point">The point to subtract to this point's axis values</param>
    /// <returns>A new point with the subtracted axis values</returns>
    public Point Subtract(Point point) => this - point;

    public static Point operator -(Point left, Point right) => new(left.X - right.X, left.Z - right.Z);
    public static Point operator -(Point left, double right) => new(left.X - right, left.Z - right);
    public static Point operator -(double left, Point right) => new(left - right.X, left - right.Z);

    /// <summary>
    /// Multiplies the specified amount to the point's axis values
    /// </summary>
    /// <param name="num">The value to multiply to the point's axis values</param>
    /// <returns>A new point with the multiplied axis values</returns>
    public Point Multiply(double num) => this * num;

    /// <summary>
    /// Multiplies the specified point's axis values to this point's axis values
    /// </summary>
    /// <param name="point">The point to multiply to this point's axis values</param>
    /// <returns>A new point with the multiplied axis values</returns>
    public Point Multiply(Point point) => this * point;

    public static Point operator *(Point left, Point right) => new(left.X * right.X, left.Z * right.Z);
    public static Point operator *(Point left, double right) => new(left.X * right, left.Z * right);
    public static Point operator *(double left, Point right) => new(left * right.X, left * right.Z);

    /// <summary>
    /// Divides the specified amount to the point's axis values
    /// </summary>
    /// <param name="num">The value to divide to the point's axis values</param>
    /// <returns>A new point with the divided axis values</returns>
    public Point Divide(double num) => this / num;

    /// <summary>
    /// Divides the specified point's axis values from this point's axis values
    /// </summary>
    /// <param name="point">The point to divide from this point's axis values</param>
    /// <returns>A new point with the divided axis values</returns>
    public Point Divide(Point point) => this / point;

    public static Point operator /(Point left, Point right) => new(left.X / right.X, left.Z / right.Z);
    public static Point operator /(Point left, double right) => new(left.X / right, left.Z / right);
    public static Point operator /(double left, Point right) => new(left / right.X, left / right.Z);

    /// <summary>Round the point values up to the nearest integer</summary>
    /// <returns>A new point with the values rounded up to the nearest integer</returns>
    public Point Ceil() => new(Math.Ceiling(X), Math.Ceiling(Z));

    /// <summary>Round the point values down to the nearest integer</summary>
    /// <returns>A new point with the values rounded down to the nearest integer</returns>
    public Point Floor() => new(Math.Floor(X), Math.Floor(Z));

    /// <summary>Round the point values to the nearest integer</summary>
    /// <returns>A new point with the values rounded to the nearest integer</returns>
    public Point Round() => new(Math.Round(X), Math.Round(Z));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Point other && Equals(other.X, other.Z);

    /// <summary>
    /// Determines whether the specified values are equal to the current object's values
    /// </summary>
    /// <param name="x">The x value to compare with the current object</param>
    /// <param name="z">The z value to compare with the current object</param>
    /// <returns><see langword="true"/> if the specified values are equal to the current object's values</returns>
    public bool Equals(double x, double z) => X.Equals(x) && Z.Equals(z);

    public static bool operator ==(Point? left, Point? right) => left?.Equals(right) ?? false;
    public static bool operator !=(Point? left, Point? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => $"{X}, {Z}";

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Z);
}
