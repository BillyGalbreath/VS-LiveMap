using System;
using System.Globalization;
using JetBrains.Annotations;
using LiveMap.Common.Api.Json;
using Newtonsoft.Json;

namespace LiveMap.Common.Api;

/// <summary>
/// Represents a 24 bit color
/// </summary>
[PublicAPI]
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color {
    private readonly uint _value;

    /// <summary>
    /// Create a new Color from string
    /// </summary>
    /// <param name="value">the uint value of the color (<c>"#RRGGBB"</c>)</param>
    public Color(string value) : this(Parse(value)) { }

    /// <summary>
    /// Create a new Color from uint
    /// </summary>
    /// <param name="value">the uint value of the color (<c>0xRRGGBB</c>)</param>
    public Color(uint value) => _value = value;

    /// <summary>
    /// Returns the uint value of this color
    /// </summary>
    /// <returns>uint color value</returns>
    public uint ToUInt() => _value;

    /// <summary>
    /// Returns the string hex value of this color
    /// </summary>
    /// <returns>string hex value</returns>
    public override string ToString() => $"#{_value:X6}";

    /// <summary>
    /// Implicit cast string to Color
    /// </summary>
    /// <param name="value">the string to cast</param>
    /// <returns>Color parsed from string</returns>
    public static implicit operator Color(string value) => new(value);

    /// <summary>
    /// Implicit cast uint to Color
    /// </summary>
    /// <param name="value">the uint to cast</param>
    /// <returns>Color parsed from uint</returns>
    public static implicit operator Color(uint value) => new(value);

    /// <summary>
    /// Implicit cast Color to string
    /// </summary>
    /// <param name="color">the Color to cast</param>
    /// <returns>string representation of this color in the format of <c>"#RRGGBB"</c></returns>
    public static implicit operator string(Color color) => color.ToString();

    /// <summary>
    /// Implicit cast Color to uint
    /// </summary>
    /// <param name="color">the Color to cast</param>
    /// <returns>uint representation of this color in the format of <c>0xRRGGBB</c></returns>
    public static implicit operator uint(Color color) => color._value;

    /// <summary>
    /// Parse a string representation of a color into a uint value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static uint Parse(string value) {
        string stripped;
        if (value.StartsWith("#")) {
            stripped = value[1..];
        } else if (value.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase)) {
            stripped = value[2..];
        } else if (value.StartsWith("&h", StringComparison.CurrentCultureIgnoreCase)) {
            stripped = value[2..];
        } else {
            stripped = value;
        }
        string lastSix = stripped[^Math.Min(stripped.Length, 6)..];
        return uint.Parse(lastSix, NumberStyles.HexNumber);
    }
}
