using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using LiveMap.Common.Util;
using LiveMap.Server.Configuration;
using LiveMap.Server.Util;
using MimeTypes;

namespace LiveMap.Server.Httpd;

public sealed class WebServer {
    private HttpListener? listener;
    private Thread? thread;
    private bool running;
    private bool stopped;
    private bool reload;

    public void Reload() {
        reload = true;
        Dispose();
    }

    public void Run() {
        if (!Config.Instance.WebServer.Enabled) {
            return;
        }

        if (!stopped && !running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        running = true;

        (thread = new Thread(_ => {
            int port = Config.Instance.WebServer.Port;

            Logger.Info(Lang.Get("logger.info.webserver-started", port));

            (listener = new HttpListener {
                Prefixes = { $"http://*:{port}/" }
            }).Start();

            while (running) {
                try {
                    HandleRequest(listener!.GetContext());
                } catch (Exception) {
                    if (stopped) {
                        Logger.Info(Lang.Get("logger.info.webserver-stopped"));
                        if (reload) {
                            reload = false;
                            stopped = false;
                        }
                    }

                    running = false;
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

        HttpListenerResponse response = context.Response;
        string filePath = Path.Combine(FileUtil.WebDir, urlLoc);

        byte[] buffer;
        if (File.Exists(filePath)) {
            response.ContentType = MimeTypeMap.GetMimeType(new FileInfo(filePath).Extension) ?? MediaTypeNames.Text.Plain;
            buffer = File.ReadAllBytes(filePath);
            response.StatusCode = 200;
        } else {
            response.ContentType = MediaTypeNames.Text.Html;
            buffer = File.ReadAllBytes(Path.Combine(FileUtil.WebDir, "404.html"));
            response.StatusCode = urlLoc.StartsWith("tiles") && urlLoc.EndsWith(".png") ? 200 : 404;
        }

        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
        response.AddHeader("Access-Control-Allow-Origin", "*");

        try {
            TimeSpan time = File.GetLastWriteTimeUtc(filePath) - DateTime.UnixEpoch;
            response.AddHeader("ETag", ((long) time.TotalMilliseconds).ToString());
        } catch (Exception) {
            // ignore
        }

        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public void Dispose() {
        stopped = true;

        listener?.Stop();
        listener = null;

        thread?.Interrupt();
        thread = null;
    }
}
