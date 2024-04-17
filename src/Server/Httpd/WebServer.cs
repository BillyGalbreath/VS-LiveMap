using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using LiveMap.Common.Util;
using LiveMap.Server.Configuration;
using MimeTypes;

namespace LiveMap.Server.Httpd;

public sealed class WebServer {
    private HttpListener? _listener;
    private Thread? _thread;
    private bool _running;
    private bool _stopped;
    private bool _reload;

    public void Reload() {
        _reload = true;
        Dispose();
        _stopped = false;
    }

    public void Run() {
        if (!Config.Instance.WebServer.Enabled) {
            return;
        }

        if (!_stopped && !_running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        _running = true;

        (_thread = new Thread(_ => {
            int port = Config.Instance.WebServer.Port;

            if (_listener == null) {
                Logger.Info(Lang.Get("logger.info.webserver-started", port));
            }

            try {
                (_listener = new HttpListener {
                    Prefixes = { $"http://*:{port}/" }
                }).Start();
            } catch (Exception e) {
                Logger.Error(Lang.Get("logger.error.webserver-start-failed"));
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
                        Logger.Info(Lang.Get("logger.info.webserver-stopped"));
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
        string filePath = Path.Combine(FileUtil.WebDir, urlLoc);

        byte[] buffer;
        if (File.Exists(filePath)) {
            response.ContentType = MimeTypeMap.GetMimeType(new FileInfo(filePath).Extension) ?? MediaTypeNames.Text.Plain;
            buffer = File.ReadAllBytes(filePath);
            response.StatusCode = 200;
        } else {
            response.ContentType = MediaTypeNames.Text.Html;
            buffer = File.ReadAllBytes(Path.Combine(FileUtil.WebDir, "404.html"));
            response.StatusCode = urlLoc.StartsWith("tiles") && (urlLoc.EndsWith(".png") || urlLoc.EndsWith(".webp")) ? 200 : 404;
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
