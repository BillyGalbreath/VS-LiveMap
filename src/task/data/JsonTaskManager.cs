using System;
using System.Collections.Generic;

namespace livemap.task.data;

public class JsonTaskManager {
    private readonly List<JsonTask> _tasks = new();

    public JsonTaskManager(LiveMap server) {
        _tasks.Add(new MarkersTask(server));
        _tasks.Add(new PlayersTask(server));
        _tasks.Add(new SettingsTask(server));
    }

    public void Tick() {
        foreach (JsonTask task in _tasks) {
            try {
                task.Tick();
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }
        }
    }

    public void Dispose() {
        foreach (JsonTask task in _tasks) {
            try {
                task.Dispose();
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }
        }
    }
}
