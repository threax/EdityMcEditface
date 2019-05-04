using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class BuildTaskManager
    {
        private Dictionary<String, Func<BuildTaskDefinition, IBuildTask>> buildTaskTypeMap = new Dictionary<string, Func<BuildTaskDefinition, IBuildTask>>();

        public void SetBuildTaskType(String name, Type type)
        {
            SetBuildTaskBuilder(name, (definition) =>
            {
                return Activator.CreateInstance(type, definition) as IBuildTask;
            });
        }

        public void SetBuildTaskBuilder(String name, Func<BuildTaskDefinition, IBuildTask> createBuildTask)
        {
            buildTaskTypeMap[name] = createBuildTask;
        }

        public IBuildTask CreateBuildTask(BuildTaskDefinition definition)
        {
            return buildTaskTypeMap[definition.Name].Invoke(definition);
        }
    }
}
