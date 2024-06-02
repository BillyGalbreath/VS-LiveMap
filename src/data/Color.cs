using System;
using System.Globalization;
using livemap.json;
using livemap.util;
using Newtonsoft.Json;

namespace livemap.data;

/// <summary>
/// Represents a 24 bit Color
/// </summary>
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color {
    private readonly uint _value;

    /// <summary>
    /// Create a new Color from string
    /// </summary>
    /// <param name="value">the uint value of the Color (<c>"#RRGGBB"</c>)</param>
    public Color(string value) : this(Parse(value)) { }

    /// <summary>
    /// Create a new Color from uint
    /// </summary>
    /// <param name="value">the uint value of the Color (<c>0xRRGGBB</c>)</param>
    public Color(uint value) => _value = value;

    /// <summary>
    /// Returns the uint value of this Color
    /// </summary>
    /// <returns>uint Color value</returns>
    public uint ToUInt() => _value;

    /// <summary>
    /// Returns the string hex value of this Color
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
    /// <returns>string representation of this Color in the format of <c>"#RRGGBB"</c></returns>
    public static implicit operator string(Color color) => color.ToString();

    /// <summary>
    /// Implicit cast Color to uint
    /// </summary>
    /// <param name="color">the Color to cast</param>
    /// <returns>uint representation of this Color in the format of <c>0xRRGGBB</c></returns>
    public static implicit operator uint(Color color) => color._value;

    /// <summary>
    /// Parse a string representation of a Color into a uint value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Color parsed from string</returns>
    public static uint Parse(string value) {
        string stripped;
        if (value.StartsWith('#')) {
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

    public uint Alpha() {
        return _value >> 24 & 0xFF;
    }

    public uint Red() {
        return _value >> 16 & 0xFF;
    }

    public uint Green() {
        return _value >> 8 & 0xFF;
    }

    public uint Blue() {
        return _value & 0xFF;
    }

    public Color Alpha(uint alpha) {
        return (alpha << 24) | (_value & 0xFFFFFF);
    }

    public static Color Reverse(Color color) {
        return color.Alpha() |
               (color.Red() << 0) |
               (color.Green() << 8) |
               (color.Blue() << 16);
    }

    public static Color Blend(Color color0, Color color1, float ratio) {
        float iRatio = 1 - ratio;
        return (color0.Alpha() << 24) |
               ((uint)((color0.Red() * ratio) + (color1.Red() * iRatio)) << 16) |
               ((uint)((color0.Green() * ratio) + (color1.Green() * iRatio)) << 8) |
               ((uint)((color0.Blue() * ratio) + (color1.Blue() * iRatio)));
    }

    public static Color LerpHsb(Color color0, Color color1, float delta) {
        float[] hsb0 = Rgb2Hsb(color0.Red(), color0.Green(), color0.Blue());
        float[] hsb1 = Rgb2Hsb(color1.Red(), color1.Green(), color1.Blue());
        return Hsb2Rgb(
            Mathf.Lerp(hsb0[0], hsb1[0], delta),
            Mathf.Lerp(hsb0[1], hsb1[1], delta),
            Mathf.Lerp(hsb0[2], hsb1[2], delta)
        );
    }

    public static float[] Rgb2Hsb(uint red, uint green, uint blue) {
        uint max = Mathf.Max(red, green, blue);
        uint min = Mathf.Min(red, green, blue);
        float diff = max - min;
        float saturation = max == 0 ? 0 : diff / max;
        float hue;
        if (saturation == 0) {
            hue = 0;
        } else {
            float delta = diff * 6;
            if (red == max) {
                hue = (green - blue) / delta;
            } else if (green == max) {
                hue = (1 / 3F) + ((blue - red) / delta);
            } else {
                hue = (2 / 3F) + ((red - green) / delta);
            }
            if (hue < 0) {
                hue++;
            }
        }
        return new[] { hue, saturation, max / 255F };
    }

    public static Color Hsb2Rgb(float hue, float saturation, float brightness) {
        if (saturation == 0) {
            return Convert(brightness, brightness, brightness);
        }
        if (brightness is < 0 or > 1) {
            throw new ArgumentOutOfRangeException(nameof(brightness), "brightness must be between 0 and 1");
        }
        if (saturation is < 0 or > 1) {
            throw new ArgumentOutOfRangeException(nameof(saturation), "saturation must be between 0 and 1");
        }
        hue -= (float)Math.Floor(hue);
        int i = (int)(6 * hue);
        float f = (6 * hue) - i;
        float p = brightness * (1 - saturation);
        float q = brightness * (1 - (saturation * f));
        float t = brightness * (1 - (saturation * (1 - f)));
        return i switch {
            0 => Convert(brightness, t, p),
            1 => Convert(q, brightness, p),
            2 => Convert(p, brightness, t),
            3 => Convert(p, q, brightness),
            4 => Convert(t, p, brightness),
            5 => Convert(brightness, p, q),
            _ => throw new Exception("impossible")
        };
    }

    private static Color Convert(float red, float green, float blue) {
        uint r = (uint)Math.Round(255 * red);
        uint g = (uint)Math.Round(255 * green);
        uint b = (uint)Math.Round(255 * blue);
        return (r << 16) | (g << 8) | b;
    }
}
