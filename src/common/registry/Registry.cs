using System.Collections.Generic;
using JetBrains.Annotations;

namespace livemap.common.registry;

[PublicAPI]
public abstract class Registry<T> : Dictionary<string, T>, Keyed where T : Keyed {
    public string Id { get; }

    protected Registry(string id) {
        Id = $"{LiveMapMod.Id}:{id}";
    }

    public bool Register(T value) {
        return Register(value.Id, value);
    }

    public bool Register(string id, T value) {
        return TryAdd(id, value);
    }

    public bool Unregister(T value) {
        return Unregister(value.Id, out _);
    }

    public bool Unregister(T value, out T? removed) {
        return Unregister(value.Id, out removed);
    }

    public bool Unregister(string id, out T? value) {
        return Remove(id, out value);
    }

    public void Dispose() {
        Clear();
    }
}
