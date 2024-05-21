using JetBrains.Annotations;
using Vintagestory.API.Datastructures;

namespace livemap.registry;

[PublicAPI]
public abstract class Registry<T> : OrderedDictionary<string, T>, Keyed where T : Keyed {
    public string Id { get; }

    protected Registry(string id) {
        Id = $"{LiveMap.Api.ModId}:{id}";
    }

    public virtual int Register(T value) {
        return Register(value.Id, value);
    }

    public virtual int Register(string id, T value) {
        return Add(id, value);
    }

    public virtual bool Unregister(T value) {
        return Unregister(value.Id);
    }

    public virtual bool Unregister(string id) {
        return Remove(id);
    }

    public virtual void Dispose() {
        Clear();
    }
}
