using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Jtd.Jtd
{
    public struct NewtonsoftAdapter : IJson
    {
        private JToken token;

        public NewtonsoftAdapter(JToken token)
        {
            this.token = token;
        }

        public bool IsNull()
        {
            return token.Type == JTokenType.Null;
        }

        public bool IsBoolean()
        {
            return token.Type == JTokenType.Boolean;
        }

        public bool IsNumber()
        {
            return token.Type == JTokenType.Float || token.Type == JTokenType.Integer;
        }

        public bool IsString()
        {
            return token.Type == JTokenType.String;
        }

        public bool IsArray()
        {
            return token.Type == JTokenType.Array;
        }

        public bool IsObject()
        {
            return token.Type == JTokenType.Object;
        }

        public bool AsBoolean()
        {
            return (bool)token;
        }

        public double AsNumber()
        {
            return (double)token;
        }

        public string AsString()
        {
            return (string)token;
        }

        public IList<IJson> AsArray()
        {
            List<IJson> elements = new List<IJson>();
            foreach (JToken element in token)
            {
                elements.Add(new NewtonsoftAdapter(element));
            }

            return elements;
        }


        public IDictionary<string, IJson> AsObject()
        {
            Dictionary<string, IJson> members = new Dictionary<string, IJson>();
            foreach (JProperty property in token)
            {
                members.Add(property.Name, new NewtonsoftAdapter(property.Value));
            }

            return members;
        }
    }
}
