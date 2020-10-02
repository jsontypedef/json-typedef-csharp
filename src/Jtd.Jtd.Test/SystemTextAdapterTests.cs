using Xunit;
using System.Text.Json;
using System.Collections.Generic;

namespace Jtd.Jtd.Test
{
    public class SystemTextAdapterTests
    {
        [Fact]
        public void TestNull()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse("null").RootElement);
            Assert.True(adapter.IsNull());
        }

        [Fact]
        public void TestBool()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse("true").RootElement);
            Assert.True(adapter.IsBoolean());
            Assert.True(adapter.AsBoolean());
        }

        [Fact]
        public void TestNumber()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse("3.14").RootElement);
            Assert.True(adapter.IsNumber());
            Assert.Equal(3.14, adapter.AsNumber());

            adapter = new SystemTextAdapter(JsonDocument.Parse("3").RootElement);
            Assert.True(adapter.IsNumber());
            Assert.Equal(3.0, adapter.AsNumber());

            adapter = new SystemTextAdapter(JsonDocument.Parse("1e100").RootElement);
            Assert.True(adapter.IsNumber());
            Assert.Equal(1e100, adapter.AsNumber());
        }

        [Fact]
        public void TestString()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse("\"foo\"").RootElement);
            Assert.True(adapter.IsString());
            Assert.Equal("foo", adapter.AsString());
        }

        [Fact]
        public void TestArray()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse("[true, 3.14, \"foo\"]").RootElement);
            Assert.True(adapter.IsArray());

            IList<IJson> list = adapter.AsArray();
            Assert.True(list[0].AsBoolean());
            Assert.Equal(3.14, list[1].AsNumber());
            Assert.Equal("foo", list[2].AsString());
        }

        [Fact]
        public void TestObject()
        {
            SystemTextAdapter adapter = new SystemTextAdapter(JsonDocument.Parse(
                "{ \"a\": true, \"b\": 3.14, \"c\": \"foo\" }").RootElement);
            Assert.True(adapter.IsObject());

            IDictionary<string, IJson> obj = adapter.AsObject();
            Assert.True(obj["a"].AsBoolean());
            Assert.Equal(3.14, obj["b"].AsNumber());
            Assert.Equal("foo", obj["c"].AsString());
        }
    }
}
