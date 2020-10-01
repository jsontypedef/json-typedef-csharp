using System.Collections.Generic;
using System.Text.Json;

namespace Jtd.Jtd
{
    public struct SystemTextAdapter : IJson
    {
        private JsonElement element;

        public SystemTextAdapter(JsonElement element)
        {
            this.element = element;
        }

        public bool IsNull()
        {
            return element.ValueKind == JsonValueKind.Null;
        }

        public bool IsBoolean()
        {
            return element.ValueKind == JsonValueKind.False ||
                element.ValueKind == JsonValueKind.True;
        }

        public bool IsNumber()
        {
            return element.ValueKind == JsonValueKind.Number;
        }

        public bool IsString()
        {
            return element.ValueKind == JsonValueKind.String;
        }

        public bool IsArray()
        {
            return element.ValueKind == JsonValueKind.Array;
        }

        public bool IsObject()
        {
            return element.ValueKind == JsonValueKind.Object;
        }

        public bool AsBoolean()
        {
            return element.GetBoolean();
        }

        public double AsNumber()
        {
            return element.GetDouble();
        }

        public string AsString()
        {
            return element.GetString();
        }

        public IList<IJson> AsArray()
        {
            List<IJson> elements = new List<IJson>();
            foreach (JsonElement subElement in element.EnumerateArray())
            {
                elements.Add(new SystemTextAdapter(subElement));
            }

            return elements;
        }

        public IDictionary<string, IJson> AsObject()
        {
            Dictionary<string, IJson> members = new Dictionary<string, IJson>();
            foreach (JsonProperty property in element.EnumerateObject())
            {
                members.Add(property.Name, new SystemTextAdapter(property.Value));
            }

            return members;
        }
    }
}
