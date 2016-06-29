using EdityMcEditface.HtmlRenderer;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is really meant to just run from within vs
            var dir = Path.GetFullPath("../../Schema");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Directory.SetCurrentDirectory(dir);

            writeSchema<PageDefinition>();
            writeSchema<LinkedContentEntry>();
            writeSchema<EdityProject>();
        }

        static void writeSchema<T>()
        {
            Type t = typeof(T);

            var schema = JsonSchema4.FromType<T>(new NJsonSchema.Generation.JsonSchemaGeneratorSettings()
            {
                DefaultEnumHandling = EnumHandling.String
            });
            var schemaData = schema.ToJson();
            using (var writer = new StreamWriter(File.Open(t.Name + ".json", FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(schemaData.ToString());
            }
        }
    }
}
