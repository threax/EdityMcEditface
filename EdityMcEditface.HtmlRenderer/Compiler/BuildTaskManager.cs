using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class BuildTaskManager
    {
        private Dictionary<String, Type> buildTaskTypeMap = new Dictionary<string, Type>();

        public void SetBuildTaskType(String name, Type type)
        {
            buildTaskTypeMap[name] = type;
        }

        public IBuildTask CreateBuildTask(BuildTaskDefinition definition)
        {
            buildTaskTypeMap.TryGetValue(definition.Name, out var type);
            return Activator.CreateInstance(type, definition) as IBuildTask;
        }
    }
}
