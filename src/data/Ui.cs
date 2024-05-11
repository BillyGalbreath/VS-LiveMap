using JetBrains.Annotations;

namespace livemap.data;

[PublicAPI]
public class Ui {
    public string Attribution { get; set; } = """<a href="https://mods.vintagestory.at/livemap" target="_blank">Livemap</a> &copy;2024""";

    public string Homepage { get; set; } = "https://mods.vintagestory.at/livemap";

    public string Title { get; set; } = "Vintage Story LiveMap";

    public string Logo { get; set; } = "LiveMap";

    public string Sidebar { get; set; } = "unpinned";
}
