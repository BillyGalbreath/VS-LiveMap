using System;
using System.Threading;
using System.Threading.Tasks;

namespace livemap.task;

public abstract class AsyncTask {
    protected readonly LiveMap _server;
    private readonly CancellationTokenSource _cts = new();

    private volatile bool _running;

    protected AsyncTask(LiveMap server) {
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
