using System;

namespace LiveMap.Common.Api;

public class Point {
    public double X { get; }
    public double Z { get; }

    public Point(double x, double z) {
        X = x;
        Z = z;
    }

    public Point Add(double num) {
        return this + num;
    }

    public Point Add(Point point) {
        return this + point;
    }

    public Point Subtract(double num) {
        return this - num;
    }

    public Point Subtract(Point point) {
        return this - point;
    }

    public Point Multiply(double num) {
        return this * num;
    }

    public Point Multiply(Point point) {
        return this * point;
    }

    public Point Divide(double num) {
        return this / num;
    }

    public Point Divide(Point point) {
        return this / point;
    }

    public Point Ceil() {
        return new Point(Math.Ceiling(X), Math.Ceiling(Z));
    }

    public Point Floor() {
        return new Point(Math.Floor(X), Math.Floor(Z));
    }

    public Point Round() {
        return new Point(Math.Round(X), Math.Round(Z));
    }

    public override bool Equals(object? obj) {
        return obj is Point other && Equals(other.X, other.Z);
    }

    public bool Equals(double x, double z) {
        return X.Equals(x) && Z.Equals(z);
    }

    public override string ToString() {
        return $"{X}, {Z}";
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Z);
    }

    public static Point operator +(Point left, Point right) {
        return new Point(left.X + right.X, left.Z + right.Z);
    }

    public static Point operator +(Point left, double right) {
        return new Point(left.X + right, left.Z + right);
    }

    public static Point operator +(double left, Point right) {
        return new Point(left + right.X, left + right.Z);
    }

    public static Point operator -(Point left, Point right) {
        return new Point(left.X - right.X, left.Z - right.Z);
    }

    public static Point operator -(Point left, double right) {
        return new Point(left.X - right, left.Z - right);
    }

    public static Point operator -(double left, Point right) {
        return new Point(left - right.X, left - right.Z);
    }

    public static Point operator *(Point left, Point right) {
        return new Point(left.X * right.X, left.Z * right.Z);
    }

    public static Point operator *(Point left, double right) {
        return new Point(left.X * right, left.Z * right);
    }

    public static Point operator *(double left, Point right) {
        return new Point(left * right.X, left * right.Z);
    }

    public static Point operator /(Point left, Point right) {
        return new Point(left.X / right.X, left.Z / right.Z);
    }

    public static Point operator /(Point left, double right) {
        return new Point(left.X / right, left.Z / right);
    }

    public static Point operator /(double left, Point right) {
        return new Point(left / right.X, left / right.Z);
    }

    public static bool operator ==(Point? left, Point? right) {
        return left?.Equals(right) ?? false;
    }

    public static bool operator !=(Point? left, Point? right) {
        return !(left == right);
    }
}
