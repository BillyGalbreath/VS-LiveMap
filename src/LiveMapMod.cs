﻿using System.Diagnostics.CodeAnalysis;
using livemap.client;
using livemap.server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace livemap;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class LiveMapMod : ModSystem {
    private static LiveMapMod? _instance;

    public static string Id => _instance!.Mod.Info.ModID;
    public static ILogger VanillaLogger => _instance!.Mod.Logger;

    private LiveMapClient? _client;
    private LiveMapServer? _server;

    public LiveMapMod() {
        _instance = this;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _client = new LiveMapClient(api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        _server = new LiveMapServer(api);
    }

    public override void Dispose() {
        _client?.Dispose();
        _client = null;

        _server?.Dispose();
        _server = null;
    }
}
