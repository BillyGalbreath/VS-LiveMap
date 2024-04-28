using JetBrains.Annotations;

namespace livemap.common.registry;

[PublicAPI]
public interface Keyed {
    public string Id { get; }
}
