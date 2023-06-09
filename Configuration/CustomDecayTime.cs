﻿using System;

namespace Pustalorc.Plugins.PlantDecay.Configuration;

[Serializable]
public sealed class CustomDecayTime
{
    public string Permission { get; set; }
    public long DecayTime { get; set; }

    public CustomDecayTime()
    {
        Permission = "";
        DecayTime = long.MaxValue;
    }
}