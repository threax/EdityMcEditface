using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.BuildTasks
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

    public class PublishMenu : BuildTask
    {
        private IFileFinder fileFinder;
        private JsonSerializer serializer;
        private String menuFile;
        private SiteBuilder siteBuilder;

        public PublishMenu(IFileFinder fileFinder, SiteBuilder siteBuilder, String menuFile, JsonSerializer jsonSerializer)
        {
            this.fileFinder = fileFinder;
            this.serializer = jsonSerializer;
            this.menuFile = menuFile;
            this.siteBuilder = siteBuilder;
        }

        public void execute()
        {
            try
            {
                MenuItem root;
                //Load menu
                using (var stream = new JsonTextReader(new StreamReader(this.fileFinder.ReadFile(menuFile))))
                {
                    root = serializer.Deserialize<MenuItem>(stream);
                }

                cleanMenuItems(root);

                using (var stream = new StreamWriter(siteBuilder.OpenOutputWriteStream(menuFile)))
                {
                    serializer.Serialize(stream, root);
                }
            }
            catch (FileNotFoundException)
            {
                //Ignore file not found exceptions and copy nothing.
            }
        }

        //This is really slow
        public void cleanMenuItems(MenuItem parent)
        {
            if (parent.Children != null)
            {
                List<MenuItem> itemsToRemove = new List<MenuItem>();

                foreach (var item in parent.Children)
                {
                    cleanMenuItems(item);

                    if (item.Link != null)
                    {
                        //Only fix absolute paths into this website
                        if (item.Link[0] == '\\' || item.Link[0] == '/')
                        {
                            PageDefinition def;

                            try
                            {
                                using (var stream = new JsonTextReader(new StreamReader(this.fileFinder.ReadFile(item.Link + ".json"))))
                                {
                                    def = serializer.Deserialize<PageDefinition>(stream);
                                }
                            }
                            catch (IOException)
                            {
                                def = new PageDefinition();
                            }

                            if (def.Hidden)
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
