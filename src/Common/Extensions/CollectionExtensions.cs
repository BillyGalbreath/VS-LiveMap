using System.Collections.Generic;

namespace LiveMap.Common.Extensions;

public static class CollectionExtensions {
    public static void AddIfNotExists<T>(this ICollection<T> collection, T value) {
        if (!collection.Contains(value)) {
            collection.Add(value);
        }
    }
}
