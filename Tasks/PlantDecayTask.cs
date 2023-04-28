using System.Threading;
using System.Threading.Tasks;
using Pustalorc.Libraries.AsyncThreadingUtils.TaskQueue.QueueableTasks;
using Rocket.Core.Utils;
using SDG.Unturned;

namespace Pustalorc.Plugins.PlantDecay.Tasks;

public delegate void PlantDecayCompleted(PlantDecayTask task);

public sealed class PlantDecayTask : QueueableTask
{
    public event PlantDecayCompleted? OnDecayCompleted;

    internal BarricadeDrop Drop { get; }

    internal PlantDecayTask(BarricadeDrop drop, long timeToDecay) : base(timeToDecay)
    {
        Drop = drop;
    }

    protected override Task Execute(CancellationToken token)
    {
        if (BarricadeManager.tryGetRegion(Drop.model, out var x, out var y, out var plant, out _))
            TaskDispatcher.QueueOnMainThread(() => BarricadeManager.destroyBarricade(Drop, x, y, plant));

        OnDecayCompleted?.Invoke(this);
        return Task.CompletedTask;
    }
}