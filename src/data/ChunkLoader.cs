using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using livemap.util;
using Microsoft.Data.Sqlite;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Common;
using Vintagestory.Common.Database;
using Vintagestory.Server;

namespace livemap.data;

[PublicAPI]
public class ChunkLoader {
    private readonly ServerMain _server;
    private readonly SqliteConnection _sqliteConn;
    private readonly ChunkDataPool _chunkDataPool;

    public ChunkLoader(ICoreServerAPI api) {
        _server = (api.World as ServerMain)!;
        // do not use server's connection, create our own to prevent issues
        (_sqliteConn = new SqliteConnection(new DbConnectionStringBuilder {
            {
                "Data Source", _server
                    .GetField<ChunkServerThread>("chunkThread")!
                    .GetField<GameDatabase>("gameDatabase")!
                    .GetField<SQLiteDBConnection>("conn")!
                    .GetField<string>("databaseFileName")!
            },
            { "Pooling", "false" },
            { "Mode", "ReadOnly" }
        }.ToString())).Open();
        _chunkDataPool = new ChunkDataPool(32, _server);
    }

    public IEnumerable<ChunkPos> GetAllServerMapRegionPositions() {
        using SqliteCommand sqlite = _sqliteConn.CreateCommand();
        sqlite.CommandText = "SELECT position, data FROM mapregion";
        using SqliteDataReader reader = sqlite.ExecuteReader();
        while (reader.Read()) {
            yield return ChunkPos.FromChunkIndex_saveGamev2((ulong)(long)reader["position"]);
        }
    }


    public IEnumerable<ChunkPos> GetAllMapChunkPositions() {
        using SqliteCommand sqlite = _sqliteConn.CreateCommand();
        sqlite.CommandText = "SELECT position FROM mapchunk";
        using SqliteDataReader reader = sqlite.ExecuteReader();
        while (reader.Read()) {
            yield return ChunkPos.FromChunkIndex_saveGamev2((ulong)(long)reader["position"]);
        }
    }

    public ServerMapRegion GetServerMapRegion(ulong position) {
        byte[]? regionData = GetTableData(position, "mapregion");
        ServerMapRegion? serverMapChunk = ServerMapRegion.FromBytes(regionData);
        return serverMapChunk;
    }

    public ServerMapChunk? GetServerMapChunk(ulong position) {
        byte[]? mapChunkData = GetTableData(position, "mapchunk");
        return mapChunkData == null ? null : ServerMapChunk.FromBytes(mapChunkData);
    }

    public ServerChunk? GetServerChunk(ulong position) {
        byte[]? chunkData = GetTableData(position, "chunk");
        ServerChunk? chunk = chunkData == null ? null : ServerChunk.FromBytes(chunkData, _chunkDataPool, _server);
        chunk?.Unpack_ReadOnly();
        return chunk;
    }

    private byte[]? GetTableData(ulong index, string name) {
        SqliteCommand sqlite = _sqliteConn.CreateCommand();
        sqlite.CommandText = $"SELECT data FROM {name} WHERE position=@pos";
        sqlite.Parameters.Add(new SqliteParameter {
            ParameterName = "pos",
            DbType = DbType.UInt64,
            Value = index
        });
        using SqliteDataReader reader = sqlite.ExecuteReader();
        return reader.Read() ? reader["data"] as byte[] : null;
    }

    public void Dispose() {
        try {
            _chunkDataPool.SlowDispose();
            _sqliteConn.Close();
        } catch (Exception) {
            // ignore
        }
    }
}
