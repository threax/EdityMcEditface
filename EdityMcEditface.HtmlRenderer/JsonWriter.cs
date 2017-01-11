using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    /// <summary>
    /// This class provides a single place to write json since json.net has so many settings.
    /// </summary>
    public static class JsonWriter
    {
        private static JsonSerializer serializer;

        static JsonWriter()
        {
            serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Converters.Add(new StringEnumConverter());
        }

        public static String Serialize(Object obj)
        {
            using (var writer = new StringWriter())
            {
                Serialize(obj, writer);
                return writer.ToString();
            }
        }

        public static void Serialize(Object obj, TextWriter writer)
        {
            serializer.Serialize(writer, obj);
        }
    }
}
