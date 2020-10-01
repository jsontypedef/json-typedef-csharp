using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Xunit;
using System;

namespace Jtd.Jtd.Test
{
    public class SystemTextValidatorTests
    {
        private static string[] SKIPPED_TESTS = {
            // These tests are skipped because System.DateTime does not support
            // leap seconds.
            "timestamp type schema - 1990-12-31T23:59:60Z",
            "timestamp type schema - 1990-12-31T15:59:60-08:00",
        };

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void TestValidation(string name, TestCase testCase)
        {
            testCase.Schema.Verify();

            // Certain tests are not supported. See SKIPPED_TESTS for an
            // explanation.
            if (Array.Exists(SKIPPED_TESTS, t => t == name))
            {
                return;
            }

            List<ValidationError> expected = new List<ValidationError>();
            foreach (TestCaseValidationError e in testCase.Errors)
            {
                expected.Add(new ValidationError {
                    InstancePath = e.InstancePath,
                    SchemaPath = e.SchemaPath,
                });
            }

            Validator validator = new Validator {};
            IList<ValidationError> actual = validator.Validate(
                testCase.Schema,
                new SystemTextAdapter(testCase.Instance));

            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            IDictionary<string, TestCase> validation =
                JsonSerializer.Deserialize<IDictionary<string, TestCase>>(
                    File.ReadAllText("../../../../json-typedef-spec/tests/validation.json"));


            IList<object[]> testCases = new List<object[]>();
            foreach (var item in validation)
            {
                testCases.Add(new object[] { item.Key, item.Value });
            }

            return testCases;
        }

        public class TestCase {
            [JsonPropertyName("schema")]
            public Schema Schema { get; set; }

            [JsonPropertyName("instance")]
            public JsonElement Instance { get; set; }

            [JsonPropertyName("errors")]
            public List<TestCaseValidationError> Errors { get; set; }
        }

        public class TestCaseValidationError {
            [JsonPropertyName("instancePath")]
            public List<string> InstancePath { get; set; }

            [JsonPropertyName("schemaPath")]
            public List<string> SchemaPath { get; set; }
        }
    }
}
