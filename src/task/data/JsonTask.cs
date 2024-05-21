using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace livemap.task.data;

[PublicAPI]
public abstract class JsonTask {
    protected readonly LiveMapServer _server;
    private readonly CancellationTokenSource _cts = new();

    private volatile bool _running;

    protected JsonTask(LiveMapServer server) {
        _server = server;
    }

    public async void Tick() {
        if (_running) {
            return;
        }

        _running = true;

        try {
            await TickAsync(_cts.Token);
        } catch (Exception e) {
            await Console.Error.WriteLineAsync(e.ToString());
        }

        _running = false;
    }

    protected abstract Task TickAsync(CancellationToken cancellationToken);

    public virtual void Dispose() {
        _cts.Cancel();
    }
}
