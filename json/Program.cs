using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        static void Main(string[] args)
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
        }
    }
}
