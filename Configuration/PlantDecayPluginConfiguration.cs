using System;
using System.Collections.Generic;
using Rocket.API;

namespace Pustalorc.Plugins.PlantDecay.Configuration;

[Serializable]
public sealed class PlantDecayPluginConfiguration : IRocketPluginConfiguration
{
    public long ScannerIntervalMs { get; set; }
    public long DefaultDecayTime { get; set; }
    public List<CustomDecayTime> CustomDecays { get; set; }

    public PlantDecayPluginConfiguration()
    {
        DefaultDecayTime = 3600;
        CustomDecays = new List<CustomDecayTime>();
    }

    public void LoadDefaults()
    {
        CustomDecays.Add(new CustomDecayTime { Permission = "vip.plant_decay", DecayTime = 7200 });
    }
}