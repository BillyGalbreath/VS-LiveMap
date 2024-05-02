using System;
using JetBrains.Annotations;
using livemap.registry;
using livemap.tile;

namespace livemap.render;

[PublicAPI]
public abstract class Renderer : Keyed {
    public string Id { get; }
    public LiveMapServer Server { get; }
    public TileImage? TileImage { get; set; }

    protected Renderer(LiveMapServer server, string id) {
        Id = id;
        Server = server;
    }

    public virtual void AllocateImage(int regionX, int regionZ) {
        TileImage = new TileImage(Server, regionX, regionZ);
    }

    public virtual void SaveImage() {
        TileImage?.Save(Id);
    }

    public virtual void CalculateShadows() {
        TileImage?.CalculateShadows();
    }

    public virtual void PostProcessRegion(int regionX, int regionZ, BlockData blockData) { }

    protected (int, int) ProcessBlock(BlockData.Data? block, int defY = 0) {
        if (block == null) {
            return (0, defY);
        }
        int id, y;
        if (Server.RenderTaskManager.RenderTask.BlocksToIgnore.Contains(block.Top)) {
            id = block.Under;
            y = block.Y - 1;
        } else {
            id = block.Top;
            y = block.Y;
        }
        return (id, y);
    }

    protected float ProcessShadow(int x, int y, int z, BlockData blockData) {
        (int _, int northwest) = ProcessBlock(blockData.Get(x - 1, z - 1), y);
        (int _, int north) = ProcessBlock(blockData.Get(x, z - 1), y);
        (int _, int west) = ProcessBlock(blockData.Get(x - 1, z), y);

        int direction = Math.Sign(y - northwest) + Math.Sign(y - north) + Math.Sign(y - west);
        int steepness = Math.Max(Math.Max(Math.Abs(y - northwest), Math.Abs(y - north)), Math.Abs(y - west));
        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
        return direction switch {
            > 0 => 1.08F + slopeFactor,
            < 0 => 0.92F - slopeFactor,
            _ => 1
        };
    }

    public virtual void Dispose() {
        TileImage?.Dispose();
    }

    public class Builder : Keyed {
        public string Id { get; }
        public Func<LiveMapServer, Renderer> Func { get; }

        public Builder(string id, Func<LiveMapServer, Renderer> func) {
            Id = id;
            Func = func;
        }
    }
}
