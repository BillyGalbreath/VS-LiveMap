using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using livemap.common.util;
using MimeTypes;

namespace livemap.server.httpd;

public sealed class WebServer {
    private readonly LiveMapServer _server;

    private HttpListener? _listener;
    private Thread? _thread;
    private bool _running;
    private bool _stopped;
    private bool _reload;

    public WebServer(LiveMapServer server) {
        _server = server;
    }

    public void Reload() {
        _reload = true;
        Dispose();
        _stopped = false;
    }

    public void Run() {
        if (!_server.Config.Httpd.Enabled) {
            return;
        }

        if (!_stopped && !_running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        _running = true;

        (_thread = new Thread(_ => {
            int port = _server.Config.Httpd.Port;

            if (_listener == null) {
                Logger.Info($"&aInternal webserver starting on port &e{port}");
            }

            try {
                (_listener = new HttpListener { Prefixes = { $"http://*:{port}/" } }).Start();
            } catch (Exception e) {
                Logger.Error("&cInternal webserver has failed to start");
                Logger.Error(e.ToString());
                _running = false;
                _stopped = true;
                Thread.CurrentThread.Interrupt();
                return;
            }

            while (_running) {
                try {
                    HandleRequest(_listener!.GetContext());
                } catch (Exception) {
                    if (_stopped) {
                        Logger.Info("&cInternal webserver has stopped");
                        if (_reload) {
                            _reload = false;
                            _stopped = false;
                        }
                    }

                    try {
                        _listener?.Stop();
                    } catch (Exception) {
                        // ignore
                    }

                    _running = false;
                    Thread.CurrentThread.Interrupt();
                }
            }
        })).Start();
    }

    private static void HandleRequest(HttpListenerContext context) {
        string? urlLoc = context.Request.Url?.LocalPath[1..];
        if (urlLoc is null or "") {
            urlLoc = "index.html";
        }

        using HttpListenerResponse response = context.Response;
        string filePath = Path.Combine(Files.WebDir, urlLoc);

        byte[] buffer;
        if (File.Exists(filePath)) {
            response.ContentType = MimeTypeMap.GetMimeType(new FileInfo(filePath).Extension) ?? MediaTypeNames.Text.Plain;
            buffer = File.ReadAllBytes(filePath);
            response.StatusCode = 200;
        } else {
            response.ContentType = MediaTypeNames.Text.Html;
            buffer = File.ReadAllBytes(Path.Combine(Files.WebDir, "404.html"));
            response.StatusCode = 404;
        }

        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
        response.AddHeader("Access-Control-Allow-Origin", "*");

        try {
            TimeSpan time = File.GetLastWriteTimeUtc(filePath) - DateTime.UnixEpoch;
            response.AddHeader("ETag", ((long)time.TotalMilliseconds).ToString());
        } catch (Exception) {
            // ignore
        }

        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public void Dispose() {
        _stopped = true;

        try {
            _listener?.Stop();
        } catch (ObjectDisposedException) { }
        _listener = null;

        _thread?.Interrupt();
        _thread = null;
    }
}
