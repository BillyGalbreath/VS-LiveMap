using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using LiveMap.Server.Configuration;
using MimeTypes;
using Vintagestory.API.Config;

namespace LiveMap.Server.Httpd;

public class WebServer {
    private readonly string liveMapPath = Path.Combine(GamePaths.DataPath, "ModData", "LiveMap");

    private HttpListener? listener;
    private Thread? thread;
    private bool running;
    private bool stopped;

    public void Run() {
        if (!stopped && !running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        running = true;

        (thread = new Thread(_ => {
            (listener = new HttpListener {
                Prefixes = { $"http://*:{Config.Port}/" }
            }).Start();

            while (running) {
                try {
                    HttpListenerContext context = listener.GetContext();
                    HandleRequest(context);
                } catch (Exception) {
                    running = false;
                    Thread.CurrentThread.Interrupt();
                }
            }
        })).Start();
    }

    private void HandleRequest(HttpListenerContext context) {
        string? urlLoc = context.Request.Url?.LocalPath[1..];
        if (urlLoc is null or "") {
            urlLoc = "index.html";
        }

        HttpListenerResponse response = context.Response;
        string filePath = Path.Combine(liveMapPath, urlLoc);
        FileInfo fileInfo = new(filePath);

        byte[] buffer;
        if (File.Exists(filePath)) {
            response.ContentType = MimeTypeMap.GetMimeType(fileInfo.Extension) ?? MediaTypeNames.Text.Plain;
            buffer = File.ReadAllBytes(filePath);
            response.StatusCode = 200;
        } else {
            response.ContentType = MediaTypeNames.Text.Html;
            buffer = File.ReadAllBytes(Path.Combine(liveMapPath, "404.html"));
            response.StatusCode = 404;
        }

        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
        response.AddHeader("Access-Control-Allow-Origin", "*");

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
