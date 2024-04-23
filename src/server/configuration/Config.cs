using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Vintagestory.API.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace livemap.server.configuration;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
public sealed class Config {
    [YamlMember(Order = 0, Description = """
                                         Settings regarding the built-in httpd web server.
                                         This is just a basic httpd web server, if you need/want
                                         something better just disable this and run your own.
                                         """)]
    public WebServerConfig WebServer = new();

    [YamlMember(Order = 1, Description = """
                                         Settings regarding the zoom aspect of the map.
                                         The more zoom levels you have the more tile sets to create
                                         which uses more drive space. So be cautious.
                                         """)]
    public ZoomConfig Zoom = new();

    [YamlMember(Order = 2, Description = """
                                         Settings regarding the logger for this mod.
                                         Only deals with log messages this mod produces.
                                         """)]
    public LoggerConfig Logger = new();

    private static string ConfigFile => Path.Combine(GamePaths.ModConfig, $"{LiveMapMod.Id}.yml");

    public static Config Instance { get; private set; } = null!;

    private static FileWatcher? _watcher;

    public static void Reload() {
        Instance = Write(Read());

        _watcher ??= new FileWatcher();

        // todo - reset whatever needs to be reset
        // write new settings.json file in tiles dir

        common.util.Logger.Info($"Loaded config from {ConfigFile}");
    }

    private static Config Read() {
        try {
            string yaml = File.ReadAllText(ConfigFile, Encoding.UTF8);
            return new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build().Deserialize<Config>(yaml);
        } catch (Exception) {
            return new Config();
        }
    }

    private static T Write<T>(T config) {
        GamePaths.EnsurePathExists(GamePaths.ModConfig);
        string yaml = new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build().Serialize(config);
        File.WriteAllText(ConfigFile, yaml, Encoding.UTF8);
        return config;
    }

    public static void Dispose() {
        _watcher?.Dispose();
        _watcher = null;
    }
}

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
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

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
public class ZoomConfig {
    [YamlMember(Order = 0, Description = """
                                         The default zoom when loading the map in browser.
                                         Normal sized tiles (1 pixel = 1 block) are always
                                         at zoom level 0, making it a good default value.
                                         """)]
    public int Default = 0;

    [YamlMember(Order = 1, Description = """
                                         Extra zoom in layers will stretch the original
                                         tile images so you can zoom in further without
                                         the extra cost of rendering more tiles.
                                         """)]
    public int MaxIn = 3;

    [YamlMember(Order = 2, Description = """
                                         The maximum zoom out you can do on the map.
                                         Each additional level requires a new set of tiles
                                         to be rendered, so don't go too wild here.
                                         """)]
    public int MaxOut = 8;
}

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
public class LoggerConfig {
    [YamlMember(Order = 0, Description = """
                                         Prints pretty colors to the console instead of just normal boring text.
                                         Disable this if your console/terminal does not support colors.
                                         """)]
    public bool ColorConsole = true;

    [YamlMember(Order = 1, Description = """
                                         Prints debug messages to the console.
                                         It will still write to the log file(s) regardless.
                                         """)]
    public bool DebugToConsole = false;

    [YamlMember(Order = 2, Description = """
                                         Prints debug messages to the event log file.
                                         It will still write to the debug log file regardless.
                                         """)]
    public bool DebugToEventFile = false;
}
