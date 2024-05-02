using System.Collections.Generic;
using JetBrains.Annotations;

namespace livemap.registry;

[PublicAPI]
public abstract class Registry<T> : Dictionary<string, T>, Keyed where T : Keyed {
    public string Id { get; }

    protected Registry(string id) {
        Id = $"{LiveMapMod.Id}:{id}";
    }

    public virtual bool Register(T value) {
        return Register(value.Id, value);
    }

    public virtual bool Register(string id, T value) {
        return TryAdd(id, value);
    }

    public virtual bool Unregister(T value) {
        return Unregister(value.Id, out _);
    }

    public virtual bool Unregister(T value, out T? removed) {
        return Unregister(value.Id, out removed);
    }

    public virtual bool Unregister(string id, out T? value) {
        return Remove(id, out value);
    }

    public virtual void Dispose() {
        Clear();
    }
}
