using System.Globalization;
using JetBrains.Annotations;
using livemap.Common.Api.Json;
using Newtonsoft.Json;

namespace LiveMap.Common.Api;

[PublicAPI]
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color {
    private readonly uint _value;

    public Color(string value) : this(uint.Parse(value[^6..], NumberStyles.HexNumber)) { }

    private Color(uint uintVal) {
        _value = uintVal;
    }

    public override string ToString() {
        return $"#{_value:X6}";
    }

    public static implicit operator Color(string value) {
        return new Color(value);
    }

    public static implicit operator Color(uint value) {
        return new Color(value);
    }

    public static implicit operator string(Color color) {
        return color.ToString();
    }

    public static implicit operator uint(Color color) {
        return color._value;
    }
}
