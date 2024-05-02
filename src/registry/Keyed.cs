using JetBrains.Annotations;

namespace livemap.registry;

[PublicAPI]
public interface Keyed {
    public string Id { get; }
}
