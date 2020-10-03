using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Jtd.Jtd
{
    /// <summary>
    /// Struct <c>NewtonsoftAdapter</c> implements <see cref="IJson" /> by
    /// wrapping a <see cref="Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// You can construct an instance of Newtonsoft's <c>JToken</c> in multiple
    /// ways; <c>JToken</c> is Newtonsoft.Json's representation of arbitrary
    /// JSON data. However you construct these instances, it is strongly
    /// recommended that you set the <c>DateParseHandling</c> setting to
    /// <c>DateParseHandling.None</c>.
    /// </para>
    ///
    /// <para>
    /// If you do not disable date parsing, then Newtonsoft.Json will lose the
    /// original format of the inputted data if it recognizes the data as a
    /// date, and will erroneously report data as valid when it is not.
    /// </para>
    ///
    /// <para>
    /// If you are constructing instances of <see
    /// cref="Newtonsoft.Json.Linq.JToken" /> using <see
    /// cref="Newtonsoft.Json.JsonConvert.DeserializeObject" />, then set <see
    /// cref="Newtonsoft.Json.JsonSerializerSettings.DateParseHandling" /> to
    /// <c>None</c>. If you're instead using <see
    /// cref="Newtonsoft.Json.Linq.JToken.ReadFrom" />, then set <see
    /// cref="P:Newtonsoft.Json.JsonTextReader.DateParseHandling" /> to
    /// <c>None</c>.
    /// </para>
    /// </remarks>
    ///
    /// <seealso cref="Validator.Validate" />
    public struct NewtonsoftAdapter : IJson
    {
        private JToken token;

        /// <summary>
        /// Constructs a <c>NewtonsoftAdapter</c> around a <c>JToken</c>.
        /// </summary>
        ///
        /// <param name="token">the token to wrap</param>
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
