﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialKHelper.SpecialKProfilesEditor.Domain
{
    public enum PluginLoadMode
    {
        [Description("Early")]
        Early,
        [Description("PlugIn")]
        PlugIn
    }
}
