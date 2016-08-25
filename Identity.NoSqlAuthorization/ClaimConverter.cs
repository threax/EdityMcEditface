using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.NoSqlAuthorization
{
    public class ClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Claim));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            return new Claim(
                jo["Type"].ToString(), 
                jo["Value"].ToString(), 
                jo["ValueType"].ToString(), 
                jo["Issuer"].ToString(), 
                jo["OriginalIssuer"].ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var claim = value as Claim;

            JObject jo = new JObject();
            jo["Type"] = claim.Type;
            jo["Value"] = claim.Value;
            jo["ValueType"] = claim.ValueType;
            jo["Issuer"] = claim.Issuer;
            jo["OriginalIssuer"] = claim.OriginalIssuer;

            jo.WriteTo(writer);
        }
    }
}
