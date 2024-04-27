using System.Collections.Generic;

namespace livemap.common.registry;

public abstract class Registry<T> : Dictionary<string, T> where T : Registry<T> {
    public string Id { get; }

    protected Registry(string id) {
        Id = $"{LiveMapMod.Id}:{id}";
    }

    public bool Register(string id, T value) {
        return TryAdd(id, value);
    }

    public bool Unregister(string id, out T? value) {
        return Remove(id, out value);
    }
}
