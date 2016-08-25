using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Identity.NoSqlAuthorization
{
    public class JsonSimpleSerializer<T> : IAuthSerializer<T>
    {
        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings();

        static JsonSimpleSerializer()
        {
            serializerSettings.Converters.Add(new ClaimConverter());
        }

        String file;

        public JsonSimpleSerializer(String file)
        {
            this.file = file;
        }

        public IEnumerable<T> Load()
        {
            if (File.Exists(file))
            {
                using (var tr = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(tr.ReadToEnd(), serializerSettings);
                }
            }
            return new T[0];
        }

        public void Save(IEnumerable<T> roles)
        {
            using(var tw = new StreamWriter(File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
            {
                var json = JsonConvert.SerializeObject(roles, serializerSettings);
                tw.Write(json);
            }
        }
    }
}
