using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using LiveMap.Server.Util;
using Vintagestory.API.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LiveMap.Server.Configuration;

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Config {
    [YamlMember(Order = 0, Description = "")]
    public WebServerConfig WebServer = new();

    public class WebServerConfig {
        [YamlMember(Order = 0, Description = """
                                             Enable the built-in http web server.
                                             Disable this if you want to manually use a
                                             standalone web server such as apache or nginx.
                                             """)]
        public bool Enabled = true;

        [YamlMember(Order = 1, Description = """
                                             The port the built-in web server listens to.
                                             Make sure the port is allocated if using a game web panel.
                                             """)]
        public int Port = 8080;
    }

    public class ZoomConfig {
        [YamlMember(Order = 0, Description = """
                                             The default zoom when loading the map in browser.
                                             Normal sized tiles (1 pixel = 1 block) are always
                                             at zoom level 0, making it a good default value.
                                             """)]
        public int Default = 0;

        [YamlMember(Order = 1, Description = """
                                             The maximum zoom out you can do on the map.
                                             Each additional level requires a new set of tiles
                                             to be rendered, so don't go too wild here.
                                             """)]
        public int MaxOut = 3;

        [YamlMember(Order = 2, Description = """
                                             Extra zoom in layers will stretch the original
                                             tile images so you can zoom in further without
                                             the extra cost of rendering more tiles.
                                             """)]
        public int MaxIn = 2;

        [YamlMember(Order = 3, Description = """
                                             Forces the map's zoom level to always be a multiple of this.
                                             By default, the zoom level snaps to the nearest integer; lower
                                             values (e.g. 0.5 or 0.1) allow for greater granularity. A
                                             value of 0 means the zoom level will not be snapped.
                                             """)]
        public double Snap = 0.25D;

        [YamlMember(Order = 4, Description = """
                                             Controls how much the map's zoom level will change after a zoom in,
                                             zoom out, pressing + or - on the keyboard, or using the zoom controls.
                                             Values smaller than 1 (e.g. 0.5) allow for greater granularity.
                                             """)]
        public double Delta = 0.25D;

        [YamlMember(Order = 5, Description = """
                                             How many scroll pixels (as reported by L.DomEvent.getWheelDelta) mean
                                             a change of one full zoom level. Smaller values will make wheel-zooming
                                             faster (and vice versa).
                                             """)]
        public int Wheel = 240;
    }

    public static Config Instance { get; private set; } = null!;

    public static void Reload() {
        Instance = Write(Read());
    }

    private static Config Read() {
        try {
            return new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build().Deserialize<Config>(File.ReadAllText(FileUtil.ConfigFile));
        } catch (Exception) {
            return new Config();
        }
    }

    private static Config Write(Config config) {
        GamePaths.EnsurePathExists(GamePaths.ModConfig);
        File.WriteAllText(FileUtil.ConfigFile,
            new SerializerBuilder()
                .WithQuotingNecessaryStrings()
                .WithNamingConvention(NullNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build().Serialize(config)
            , Encoding.UTF8);
        return config;
    }
}
