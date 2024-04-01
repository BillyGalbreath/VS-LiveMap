using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using LiveMap.Common.Extensions;
using Microsoft.Data.Sqlite;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Common;
using Vintagestory.Common.Database;
using Vintagestory.Server;

namespace LiveMap.Server.Util;

public class DataLoader {
    private readonly ServerMain _server;
    private readonly SqliteConnection _sqliteConn;
    private readonly ChunkDataPool _chunkDataPool;

    public DataLoader(ICoreServerAPI api) {
        _server = (api.World as ServerMain)!;
        string filename = _server
            .GetField<ChunkServerThread>("chunkThread")!
            .GetField<GameDatabase>("gameDatabase")!
            .GetField<SQLiteDBConnection>("conn")!
            .GetField<string>("databaseFileName")!;
        /*_sqliteConn = _server
            .GetField<ChunkServerThread>("chunkThread")!
            .GetField<GameDatabase>("gameDatabase")!
            .GetField<SQLiteDBConnection>("conn")!
            .GetField<SqliteConnection>("sqliteConn")!;*/
        _sqliteConn = new SqliteConnection(new DbConnectionStringBuilder {
            { "Data Source", filename },
            { "Pooling", "false" },
            { "Mode", "ReadOnly" }
        }.ToString());
        _sqliteConn.Open();
        _chunkDataPool = new ChunkDataPool(32, _server);
    }

    public IEnumerable<ChunkPos> GetAllMapChunkPositions() {
        using SqliteCommand cmd = _sqliteConn.CreateCommand();
        cmd.CommandText = "SELECT position FROM mapchunk";
        using SqliteDataReader reader = cmd.ExecuteReader();
        while (reader.Read()) {
            yield return ChunkPos.FromChunkIndex_saveGamev2((ulong)(long)reader["position"]);
        }
    }

    public ServerMapChunk? GetServerMapChunk(int x, int y, int z) {
        return GetServerMapChunk(ChunkPos.ToChunkIndex(x, y, z));
    }

    public ServerMapChunk? GetServerMapChunk(ChunkPos position) {
        return GetServerMapChunk(ChunkPos.ToChunkIndex(position.X, position.Y, position.Z));
    }

    private ServerMapChunk? GetServerMapChunk(ulong position) {
        byte[]? mapChunk = GetChunk(position, "mapchunk");
        return mapChunk == null ? null : ServerMapChunk.FromBytes(mapChunk);
    }

    public ServerChunk? GetServerChunk(int x, int y, int z) {
        return GetServerChunk(ChunkPos.ToChunkIndex(x, y, z));
    }

    private ServerChunk? GetServerChunk(ulong position) {
        byte[]? data = GetChunk(position, "chunk");
        if (data == null) {
            return null;
        }

        ServerChunk chunk = ServerChunk.FromBytes(data, _chunkDataPool, _server);
        chunk?.Unpack_ReadOnly();
        return chunk;
    }

    private byte[]? GetChunk(ulong position, string tableName) {
        SqliteCommand cmd = _sqliteConn.CreateCommand();
        cmd.CommandText = "SELECT data FROM @tableName WHERE position=@position";
        cmd.Parameters.Add(CreateParameter("tableName", DbType.String, tableName, cmd));
        cmd.Parameters.Add(CreateParameter("position", DbType.UInt64, position, cmd));
        using SqliteDataReader dataReader = cmd.ExecuteReader();
        return dataReader.Read() ? dataReader["data"] as byte[] : null;
    }

    private static DbParameter CreateParameter(string parameterName, DbType dbType, object? value, DbCommand command) {
        DbParameter dbParameter = command.CreateParameter();
        dbParameter.ParameterName = parameterName;
        dbParameter.DbType = dbType;
        dbParameter.Value = value;
        return dbParameter;
    }
}
