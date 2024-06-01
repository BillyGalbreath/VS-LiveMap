using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using livemap.configuration;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.util;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace livemap.layer.builtin;

public class TradersLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override List<Marker> Markers {
        get {
            List<Marker> list = new();
            _knownTraders.Values.Foreach(traders => {
                traders.Foreach(trader => {
                    TooltipOptions? tooltip = Config.Tooltip?.DeepCopy();
                    if (tooltip?.Content != null) {
                        tooltip.Content = string.Format(tooltip.Content, trader.Name, trader.Type);
                    }
                    PopupOptions? popup = Config.Popup?.DeepCopy();
                    if (popup?.Content != null) {
                        popup.Content = string.Format(popup.Content, trader.Name, trader.Type);
                    }
                    string id = $"trader:{trader.Pos.X},{trader.Pos.Y},{trader.Pos.Z}";
                    list.Add(new Icon(id, trader.Pos.ToPoint(), Config.IconOptions) {
                        Tooltip = tooltip,
                        Popup = popup
                    });
                });
            });
            return list;
        }
    }

    public override string? Css => Config.Css;

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    private static Traders Config => LiveMap.Api.Config.Layers.Traders;

    private readonly ConcurrentDictionary<ulong, HashSet<Trader>> _knownTraders = new();

    public TradersLayer() : base("traders", "lang.traders".ToLang()) { }

    public void SetTraders(ulong chunkIndex, HashSet<Trader> traders) {
        _knownTraders[chunkIndex] = traders;
    }

    public class Trader {
        public readonly string Type;
        public readonly string? Name;
        public readonly BlockPos Pos;

        public Trader(string type, string? name, BlockPos pos) {
            Type = type;
            Name = name;
            Pos = pos;
        }
    }
}
