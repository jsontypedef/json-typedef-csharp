using Xunit;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Jtd.Jtd.Test
{
    public class NewtonsoftSchemaTests
    {
        [Fact]
        public void TestSerialize()
        {
            Schema schema = new Schema();
            schema.Metadata = new Dictionary<string, object>() { { "foo", "bar" } };
            schema.Nullable = false;
            schema.Definitions = new Dictionary<string, Schema>() { { "foo", new Schema() } };
            schema.Ref = "foo";
            schema.Type = Type.Uint8;
            schema.Enum = new HashSet<string>() { "" };
            schema.Elements = new Schema();
            schema.Properties = new Dictionary<string, Schema>() { { "foo", new Schema() } };
            schema.OptionalProperties = new Dictionary<string, Schema>() { { "foo", new Schema() } };
            schema.AdditionalProperties = true;
            schema.Values = new Schema();
            schema.Discriminator = "foo";
            schema.Mapping = new Dictionary<string, Schema>() { { "foo", new Schema() } };

            string serialized = JsonConvert.SerializeObject(schema, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
            });

            Assert.Equal(
                @"{""metadata"":{""foo"":""bar""},""nullable"":false,""definitions"":{""foo"":{}},""ref"":""foo"",""type"":""uint8"",""enum"":[""""],""elements"":{},""properties"":{""foo"":{}},""optionalProperties"":{""foo"":{}},""additionalProperties"":true,""values"":{},""discriminator"":""foo"",""mapping"":{""foo"":{}}}",
                serialized
            );
        }

        private static string[] SKIPPED_TESTS = {
            // These tests are skipped because Newtonsoft.Json does not support
            // disabling its behavior of automatically "casting" JSON data to
            // the appropriate CSharp type.
            "nullable not boolean",
            "type not string",
            "enum not array of strings",
            "additionalProperties not boolean",
            "discriminator not string",

            // This test is skipped because Newtonsoft.Json automatically
            // dedupes data when creating an ISet, so the information about
            // duplicate data is lost.
            "enum contains duplicates",
        };

        [Theory]
        [MemberData(nameof(GetInvalidSchemas))]
        public void CanDetectInvalidSchemas(string message, JToken json)
        {
            // We skip certain tests that Newtonsoft.Json cannot easily
            // accomodate. See SKIPPED_TESTS for an explanation of each case.
            if (Array.Exists(SKIPPED_TESTS, t => t == message))
            {
                return;
            }

            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            });

            try
            {
                Schema schema = json.ToObject<Schema>(serializer);

                // For the input json "null", schema will be serialized as null.
                // That's the desired behavior. We shouldn't try to call Verify
                // on schema in this case.
                if (schema != null)
                {
                    Assert.Throws<InvalidSchemaException>(() => { schema.Verify(); });
                }
            }
            catch (JsonSerializationException)
            {
                // This error is ok as well.
            }
        }

        public static IEnumerable<object[]> GetInvalidSchemas()
        {
            IDictionary<string, JToken> invalidSchemas =
                JsonConvert.DeserializeObject<IDictionary<string, JToken>>(
                    File.ReadAllText("../../../../../json-typedef-spec/tests/invalid_schemas.json"));

            IList<object[]> testCases = new List<object[]>();
            foreach (var item in invalidSchemas)
            {
                testCases.Add(new object[] { item.Key, item.Value });
            }

            return testCases;
        }
    }
}
