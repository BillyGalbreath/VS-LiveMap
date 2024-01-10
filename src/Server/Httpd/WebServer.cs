using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
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
            while (running) {
                try {
                    listener = new HttpListener();
                    listener.Prefixes.Add("http://localhost:8080/");
                    listener.Start();

                    while (listener.IsListening) {
                        listener
                            .BeginGetContext(Callback, listener)
                            .AsyncWaitHandle
                            .WaitOne();
                    }
                } catch (ThreadInterruptedException) {
                    Thread.CurrentThread.Interrupt();
                }

                running = false;
            }
        })).Start();
    }

    private void Callback(IAsyncResult result) {
        HttpListenerContext ctx = ((HttpListener)result.AsyncState!).EndGetContext(result);

        string? urlLoc = ctx.Request.Url?.LocalPath[1..];
        if (urlLoc is null or "") {
            urlLoc = "index.html";
        }

        HttpListenerResponse response = ctx.Response;
        string filePath = Path.Combine(liveMapPath, urlLoc);

        byte[] buffer;
        if (File.Exists(filePath)) {
            FileInfo fileInfo = new(filePath);
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
