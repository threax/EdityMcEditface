﻿using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Newtonsoft.Json;
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
        private IFileFinder fileFinder;
        private JsonSerializer serializer;
        private String menuFile;
        private ISiteBuilder siteBuilder;

        public PublishMenu(IFileFinder fileFinder, ISiteBuilder siteBuilder, String menuFile, JsonSerializer jsonSerializer)
        {
            this.fileFinder = fileFinder;
            this.serializer = jsonSerializer;
            this.menuFile = menuFile;
            this.siteBuilder = siteBuilder;
        }

        public Task Execute()
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
            return Task.FromResult(0);
        }

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