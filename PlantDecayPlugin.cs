using Pustalorc.Plugins.AsynchronousTaskDispatcher.Dispatcher;
using Pustalorc.Plugins.PlantDecay.Configuration;
using Pustalorc.Plugins.PlantDecay.Tasks;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace Pustalorc.Plugins.PlantDecay;

public sealed class PlantDecayPlugin : RocketPlugin<PlantDecayPluginConfiguration>
{
    private PlantScannerTask ScannerTask { get; }

    public PlantDecayPlugin()
    {
        ScannerTask = new PlantScannerTask(this);
    }

    protected override void Load()
    {
        ScannerTask.ResetTimestamp();

        if (!Level.isLoaded)
            Level.onPostLevelLoaded += OnLevelLoaded;
        else
            OnLevelLoaded(0);

        Logger.Log("Plant Decay Plugin v1.0.1, by Pustalorc has been loaded!");
    }

    private void OnLevelLoaded(int level)
    {
        AsyncTaskDispatcher.QueueTask(ScannerTask);
    }

    protected override void Unload()
    {
        ScannerTask.Cancel();

        Logger.Log("Plant Decay Plugin v1.0.1, by Pustalorc has been unloaded!");
    }
}