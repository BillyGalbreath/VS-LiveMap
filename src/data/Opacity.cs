using System;
using JetBrains.Annotations;
using livemap.json;
using Newtonsoft.Json;

namespace livemap.data;

[PublicAPI]
[JsonConverter(typeof(OpacityJsonConverter))]
public readonly struct Opacity {
    private readonly byte _value;

    public Opacity(double value) : this((byte)(0xFF * value)) { }
    public Opacity(byte value) => _value = value;

    public byte ToByte() => _value;
    public double ToDouble() => Math.Round((double)_value / 0xFF, 1);
    public override string ToString() => $"0x{_value:X2}";

    public override int GetHashCode() => _value.GetHashCode();
    public override bool Equals(object? obj) => obj is double or byte or Opacity && ((Opacity)obj)._value == _value;
    public static bool operator ==(Opacity? left, Opacity? right) => left?.Equals(right) ?? false;
    public static bool operator !=(Opacity? left, Opacity? right) => !(left == right);

    public static implicit operator Opacity(double value) => new(value);
    public static implicit operator Opacity(byte value) => new(value);

    public static implicit operator double(Opacity opacity) => opacity.ToDouble();
    public static implicit operator byte(Opacity opacity) => opacity.ToByte();
}
