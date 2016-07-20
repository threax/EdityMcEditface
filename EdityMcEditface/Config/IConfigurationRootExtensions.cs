using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationRootExtensions
    {
        public static bool getVal(this IConfigurationRoot config, String name, bool defaultVal = false)
        {
            bool readSettingsFromCurrent;
            if (!bool.TryParse(config[name], out readSettingsFromCurrent))
            {
                readSettingsFromCurrent = defaultVal;
            }
            return readSettingsFromCurrent;
        }
    }
}
