using Xunit;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace Jtd.Jtd.Test
{
    public class SystemTextSchemaTests
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

            string serialized = JsonSerializer.Serialize(schema, new JsonSerializerOptions{
                IgnoreNullValues = true,
            });

            Assert.Equal(
                @"{""metadata"":{""foo"":""bar""},""nullable"":false,""definitions"":{""foo"":{}},""ref"":""foo"",""type"":""uint8"",""enum"":[""""],""elements"":{},""properties"":{""foo"":{}},""optionalProperties"":{""foo"":{}},""additionalProperties"":true,""values"":{},""discriminator"":""foo"",""mapping"":{""foo"":{}}}",
                serialized
            );
        }

        private static string[] SKIPPED_TESTS = {
            // System.Text.Json does not support rejecting missing / extra
            // properties:
            //
            // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to#missingmemberhandling
            "illegal keyword",

            // System.Text.Json.Serialization.JsonStringEnumConverter supports
            // letting an integer like "123" become some non-existent 124th
            // member of an enum (i.e. an enum member with integer
            // representation 123).
            //
            // To make this test pass, we would have to have Schema check its
            // own Type attribute to make sure its value is a valid Type. At the
            // time of writing, this seems not to be worth it.
            "type not string",

            // This test is skipped because System.Text.Json automatically
            // dedupes data when creating an ISet, so the information about
            // duplicate data is lost.
            "enum contains duplicates",
        };

        [Theory]
        [MemberData(nameof(GetInvalidSchemas))]
        public void CanDetectInvalidSchemas(string message, JsonElement json)
        {
            // We skip certain tests that Newtonsoft.Json cannot easily
            // accomodate. See SKIPPED_TESTS for an explanation of each case.
            if (Array.Exists(SKIPPED_TESTS, t => t == message))
            {
                return;
            }

            try
            {
                // It is a known issue in System.Text.Json that you have to go
                // back and forth with UTF-8 to do this conversion:
                //
                // https://github.com/dotnet/runtime/issues/31274
                Schema schema = JsonSerializer.Deserialize<Schema>(json.GetRawText());

                // For the input json "null", schema will be serialized as null.
                // That's the desired behavior. We shouldn't try to call Verify
                // on schema in this case.
                if (schema != null)
                {
                    Assert.Throws<InvalidSchemaException>(() => { schema.Verify(); });
                }
            }
            catch (JsonException)
            {
                // This error is ok as well.
            }
        }

        public static IEnumerable<object[]> GetInvalidSchemas()
        {
            IDictionary<string, JsonElement> invalidSchemas =
                JsonSerializer.Deserialize<IDictionary<string, JsonElement>>(
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
