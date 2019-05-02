using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    public class MenuItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Link { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Target { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MenuItem> Children { get; set; }
    }

    public class PublishMenu : IBuildTask
    {
        private JsonSerializer serializer;
        private String menuFile;

        public PublishMenu(BuildTaskDefinition definition)
        {
            if (!definition.Settings.ContainsKey("menuFile"))
            {
                throw new InvalidOperationException("You must have a setting named 'menuFile' in your build task's settings that points to the menu to use the PublishMenu task.");
            }
            this.menuFile = definition.Settings["menuFile"] as String;
            this.serializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            this.serializer.Converters.Add(new StringEnumConverter());
        }

        public Task Execute(BuildEventArgs args)
        {
            args.Tracker.AddMessage($"Publishing menu {menuFile}.");

            try
            {
                MenuItem root;
                //Load menu
                using (var stream = new JsonTextReader(new StreamReader(args.SiteBuilder.OpenInputReadStream(menuFile))))
                {
                    root = serializer.Deserialize<MenuItem>(stream);
                }

                cleanMenuItems(root, args.SiteBuilder);

                using (var stream = new StreamWriter(args.SiteBuilder.OpenOutputWriteStream(menuFile)))
                {
                    serializer.Serialize(stream, root);
                }
            }
            catch (FileNotFoundException)
            {
                //Ignore file not found exceptions and copy nothing.
            }
            return Task.FromResult(0);
        }

        private void cleanMenuItems(MenuItem parent, ISiteBuilder siteBuilder)
        {
            if (parent.Children != null)
            {
                List<MenuItem> itemsToRemove = new List<MenuItem>();

                foreach (var item in parent.Children)
                {
                    cleanMenuItems(item, siteBuilder);

                    if (item.Link != null)
                    {
                        //Only remove absolute paths into this website, also don't remove paths pointing to the site root
                        if (item.Link.Length > 1 && (item.Link[0] == '\\' || item.Link[0] == '/'))
                        {
                            if (!siteBuilder.DoesOutputFileExist(item.Link + ".html"))
                            {
                                itemsToRemove.Add(item);
                            }
                        }
                    }
                    else //No link, is a folder, check to see if its empty
                    {
                        if (item.Children == null || item.Children.Count == 0)
                        {
                            itemsToRemove.Add(item);
                        }
                    }
                }

                foreach (var remove in itemsToRemove)
                {
                    parent.Children.Remove(remove);
                }
            }
        }
    }
}
