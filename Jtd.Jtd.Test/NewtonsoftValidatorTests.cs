using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Xunit;
using System;

namespace Jtd.Jtd.Test
{
    public class NewtonsoftValidatorTests
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

            Validator validator = new Validator {};
            IList<ValidationError> actual = validator.Validate(
                testCase.Schema,
                new NewtonsoftAdapter(testCase.Instance));

            Assert.Equal(testCase.Errors, actual);
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            string input = File.ReadAllText("../../../../json-typedef-spec/tests/validation.json");

            IDictionary<string, TestCase> validation =
                JsonConvert.DeserializeObject<IDictionary<string, TestCase>>(
                    input, new JsonSerializerSettings {
                        DateParseHandling = DateParseHandling.None,
                    });

            IList<object[]> testCases = new List<object[]>();
            foreach (var item in validation)
            {
                testCases.Add(new object[] { item.Key, item.Value });
            }

            return testCases;
        }

        public class TestCase {
            public Schema Schema { get; set; }
            public JToken Instance { get; set; }
            public List<ValidationError> Errors { get; set; }
        }
    }
}
