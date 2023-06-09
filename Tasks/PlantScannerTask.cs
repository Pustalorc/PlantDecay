﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pustalorc.Libraries.AsyncThreadingUtils.TaskQueue.QueueableTasks;
using Pustalorc.Plugins.AsynchronousTaskDispatcher.Dispatcher;
using Rocket.API;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace Pustalorc.Plugins.PlantDecay.Tasks;

internal sealed class PlantScannerTask : QueueableTask
{
    private PlantDecayPlugin Plugin { get; }
    private Dictionary<uint, PlantDecayTask> DecayTasks { get; }

    internal PlantScannerTask(PlantDecayPlugin plugin) : base(plugin.Configuration.Instance.ScannerIntervalMs)
    {
        Plugin = plugin;
        DecayTasks = new Dictionary<uint, PlantDecayTask>();

        IsRepeating = true;
    }

    public void RestartTask()
    {
        IsCancelled = false;
        ResetTimestamp();
    }

    protected override Task Execute(CancellationToken token)
    {
        var plantsToDecay = BarricadeManager.regions.Cast<BarricadeRegion>().SelectMany(k => k.drops).ToList()
            .Where(k => k.interactable is InteractableFarm { IsFullyGrown: true }).ToList();

        var configuration = Plugin.Configuration.Instance;
        foreach (var plantToDecay in plantsToDecay)
        {
            if (DecayTasks.ContainsKey(plantToDecay.instanceID))
                continue;

            var ownerId = plantToDecay.GetServersideData().owner.ToString();
            var decayTime =
                configuration.CustomDecays.Find(k => new RocketPlayer(ownerId).HasPermission(k.Permission))
                    ?.DecayTime ?? configuration.DefaultDecayTime;

            var task = new PlantDecayTask(plantToDecay, decayTime * 1000);
            task.OnDecayCompleted += DecayCompleted;
            DecayTasks.Add(plantToDecay.instanceID, task);
            AsyncTaskDispatcher.QueueTask(task);
        }

        return Task.CompletedTask;
    }

    private void DecayCompleted(PlantDecayTask task)
    {
        task.OnDecayCompleted -= DecayCompleted;
        DecayTasks.Remove(task.Drop.instanceID);
    }
}