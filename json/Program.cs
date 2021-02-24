using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace json
{
    // an enum type
    public enum DayEnum
    {
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
    }


    // a custom converter
    public class YesNoConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var b = (bool)value;
            switch (b)
            {
                case true:
                    writer.WriteValue("ja");
                    break;
                case false:
                    writer.WriteValue("nein");
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = (string)reader.Value;
            switch (str)
            {
                case "ja":
                    return true;
                case "nein":
                    return false;
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    // a serialization data type
    public class MyData
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Boolean { get; set; }
        [JsonProperty("JaNein")]  // use a custom key, not the member name as json key
        [JsonConverter(typeof(YesNoConverter))]  // use a custom converter
        public bool YesNo { get; set; }
        public DateTime Time { get; set; }
        public DayEnum Day { get; set; }
        public string MaybeNull { get; set; }
    }


    class Program
    {
        // parse json representing MyData
        static private void parseMyData()
        {
            var data = new MyData { Name = "Cesar", Age = 100, Boolean = true, YesNo = false, Time = DateTime.UtcNow, Day = DayEnum.Saturday };
            // default formating
            Console.WriteLine(JsonConvert.SerializeObject(data));
            // pretty formating and ignoring null values
            Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            var data2 = new MyData { Name = "umlauts with quotation marks \"ÄÖÜ\"", Age = 100, Boolean = true, YesNo = false, Time = DateTime.UtcNow, Day = DayEnum.Saturday };
            // pretty formating, special handling of non-asci characters, and ignoring null values
            Console.WriteLine(JsonConvert.SerializeObject(data2, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            }));

            // read serialized data into new object
            var str = JsonConvert.SerializeObject(data2);
            var obj = JObject.Parse(str);
            var data3 = obj.ToObject<MyData>();
            Console.WriteLine(JsonConvert.SerializeObject(data3, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            }));

            // Newtonsoft.Json ignores case for de-serialization
            // see also https://makolyte.com/csharp-case-sensitivity-in-json-deserialization/
            string jsonStr = "{\"nAme\": \"Cesar\", \"AgE\": 100, \"BoOlean\": true, \"JaNein\": \"nein\", \"TimE\": \"2021-02-24T22:05:23.3192154Z\", \"Day\": 5}";
            var data4 = JObject.Parse(jsonStr).ToObject<MyData>();
            data4.MaybeNull = "not null";
            Console.WriteLine(JsonConvert.SerializeObject(data4, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver{
                    // use "camelCase" for serialization, note serialization of "MaybeNull"
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            }));

        }


        private static void parseToken(JToken token)
        {
            foreach (var v in token.Values())
            {
                switch (v.Type)
                {
                    // a high-quality implementation needs to cover other cases as well
                    case JTokenType.Integer:
                        {
                            var value = v.Value<Int64>();
                            Console.WriteLine($"path: {token.Path}, value: {value}");
                            break;
                        }
                    case JTokenType.Float:
                        {
                            var value = v.Value<double>();
                            Console.WriteLine($"path: {token.Path}, value: {value}");
                            break;
                        }
                    case JTokenType.Boolean:
                        {
                            var value = v.Value<bool>();
                            Console.WriteLine($"path: {token.Path}, value: {value}");
                            break;
                        }
                    case JTokenType.String:
                        {
                            var value = v.Value<string>();
                            Console.WriteLine($"path: {token.Path}, value: {value}");
                            break;
                        }
                    case JTokenType.Property:
                        parseToken(v);
                        break;
                }
            }
        }


        // parse json of unknown structure (implementation is very basic and does not cover all cases of valid json)
        private static void parseUnKnown()
        {
            string json = "{ command: \"ls\", arguments: [ \"--all\", \"--color\" ], isUnix: true, object: { pi: 3.14, name: \"Pi\" } }";
            var obj = JObject.Parse(json);
            foreach (var token in obj.Children())
            {
                parseToken(token);
            }
        }


        static void Main(string[] args)
        {
            parseMyData();
            parseUnKnown();
        }
    }
}
