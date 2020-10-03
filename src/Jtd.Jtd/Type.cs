using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jtd.Jtd
{
    /// <summary>
    /// Enum <c>Type</c> represents the values of the <c>type</c> keyword in
    /// JSON Type Definition.
    /// </summary>
    ///
    /// <seealso cref="P:Schema.Type" />
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(TypeJsonConverter))]
    public enum Type
    {
        /// <summary>The <c>boolean</c> type.</summary>
        [EnumMember(Value = "boolean")]
        Boolean,

        /// <summary>The <c>float32</c> type.</summary>
        [EnumMember(Value = "float32")]
        Float32,

        /// <summary>The <c>float64</c> type.</summary>
        [EnumMember(Value = "float64")]
        Float64,

        /// <summary>The <c>int8</c> type.</summary>
        [EnumMember(Value = "int8")]
        Int8,

        /// <summary>The <c>uint8</c> type.</summary>
        [EnumMember(Value = "uint8")]
        Uint8,

        /// <summary>The <c>int16</c> type.</summary>
        [EnumMember(Value = "int16")]
        Int16,

        /// <summary>The <c>uint16</c> type.</summary>
        [EnumMember(Value = "uint16")]
        Uint16,

        /// <summary>The <c>int32</c> type.</summary>
        [EnumMember(Value = "int32")]
        Int32,

        /// <summary>The <c>uint32</c> type.</summary>
        [EnumMember(Value = "uint32")]
        Uint32,

        /// <summary>The <c>string</c> type.</summary>
        [EnumMember(Value = "string")]
        String,

        /// <summary>The <c>timestamp</c> type.</summary>
        [EnumMember(Value = "timestamp")]
        Timestamp,
    }

    /// <summary>
    /// Class <c>TypeJsonConverter</c> is used internally by <see cref="Type"
    /// />. For technical reasons, this class must be publicly exposed. It is
    /// not meant to be used by users of Jtd.Jtd, and may change without notice.
    /// </summary>
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
