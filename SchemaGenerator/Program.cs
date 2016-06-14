using EdityMcEditface.HtmlRenderer;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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

            JSchemaGenerator generator = new JSchemaGenerator();
            generator.DefaultRequired = Newtonsoft.Json.Required.Default;

            // change contract resolver so property names are camel case
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            writeSchema<PageDefinition>(generator);
            writeSchema<LinkedContentEntry>(generator);
            writeSchema<EdityProject>(generator);
        }

        static void writeSchema<T>(JSchemaGenerator generator)
        {
            Type t = typeof(T);
            JSchema schema = generator.Generate(t);
            using(var writer = new StreamWriter(File.Open(t.Name + ".json", FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(schema.ToString());
            }
        }
    }
}
