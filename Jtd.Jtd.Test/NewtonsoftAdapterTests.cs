using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Jtd.Jtd.Test
{
    public class NewtonsoftAdapterTests
    {
        [Fact]
        public void TestNull()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse("null"));
            Assert.True(adapter.IsNull());
        }

        [Fact]
        public void TestBool()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse("true"));
            Assert.True(adapter.IsBoolean());
            Assert.True(adapter.AsBoolean());
        }

        [Fact]
        public void TestNumber()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse("3.14"));
            Assert.True(adapter.IsNumber());
            Assert.Equal(3.14, adapter.AsNumber());

            adapter = new NewtonsoftAdapter(JToken.Parse("3"));
            Assert.True(adapter.IsNumber());
            Assert.Equal(3.0, adapter.AsNumber());

            adapter = new NewtonsoftAdapter(JToken.Parse("1e100"));
            Assert.True(adapter.IsNumber());
            Assert.Equal(1e100, adapter.AsNumber());
        }

        [Fact]
        public void TestString()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse("\"foo\""));
            Assert.True(adapter.IsString());
            Assert.Equal("foo", adapter.AsString());

            JsonTextReader reader = new JsonTextReader(new StringReader("\"1985-04-12T23:20:50.52Z\""));
            reader.DateParseHandling = DateParseHandling.None;

            adapter = new NewtonsoftAdapter(JToken.ReadFrom(reader));
            Assert.True(adapter.IsString());
            Assert.Equal("1985-04-12T23:20:50.52Z", adapter.AsString());
        }

        [Fact]
        public void TestArray()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse("[true, 3.14, \"foo\"]"));
            Assert.True(adapter.IsArray());

            IList<IJson> list = adapter.AsArray();
            Assert.True(list[0].AsBoolean());
            Assert.Equal(3.14, list[1].AsNumber());
            Assert.Equal("foo", list[2].AsString());
        }

        [Fact]
        public void TestObject()
        {
            NewtonsoftAdapter adapter = new NewtonsoftAdapter(JToken.Parse(
                "{ \"a\": true, \"b\": 3.14, \"c\": \"foo\" }"));
            Assert.True(adapter.IsObject());

            IDictionary<string, IJson> obj = adapter.AsObject();
            Assert.True(obj["a"].AsBoolean());
            Assert.Equal(3.14, obj["b"].AsNumber());
            Assert.Equal("foo", obj["c"].AsString());
        }
    }
}
