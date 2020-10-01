using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jtd.Jtd
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(TypeJsonConverter))]
    public enum Type
    {
        [EnumMember(Value = "boolean")]
        Boolean,

        [EnumMember(Value = "float32")]
        Float32,

        [EnumMember(Value = "float64")]
        Float64,

        [EnumMember(Value = "int8")]
        Int8,

        [EnumMember(Value = "uint8")]
        Uint8,

        [EnumMember(Value = "int16")]
        Int16,

        [EnumMember(Value = "uint16")]
        Uint16,

        [EnumMember(Value = "int32")]
        Int32,

        [EnumMember(Value = "uint32")]
        Uint32,

        [EnumMember(Value = "string")]
        String,

        [EnumMember(Value = "timestamp")]
        Timestamp,
    }

    public class TypeJsonConverter : JsonConverterFactory
    {
        private JsonConverterFactory converterFactory;

        public TypeJsonConverter()
        {
            converterFactory = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false);
        }

        public override bool CanConvert(System.Type typeToConvert)
        {
            return converterFactory.CanConvert(typeToConvert);
        }

        public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, JsonSerializerOptions options)
        {
            return converterFactory.CreateConverter(typeToConvert, options);
        }
    }
}
