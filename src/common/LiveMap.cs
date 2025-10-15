namespace livemap.common;

public abstract class LiveMap : IDisposable {
    private readonly LiveMapModSystem _mod;

    protected LiveMap(LiveMapModSystem mod) {
        _mod = mod;
    }

    public abstract void Dispose();
}
