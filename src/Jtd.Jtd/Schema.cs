using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Jtd.Jtd
{
    public class Schema
    {
        [JsonProperty("metadata")]
        [JsonPropertyName("metadata")]
        public IDictionary<string, object> Metadata { get; set; }

        [JsonProperty("nullable")]
        [JsonPropertyName("nullable")]
        public bool? Nullable { get; set; }

        [JsonProperty("definitions")]
        [JsonPropertyName("definitions")]
        public IDictionary<string, Schema> Definitions { get; set; }

        [JsonProperty("ref")]
        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public Type? Type { get; set; }

        [JsonProperty("enum")]
        [JsonPropertyName("enum")]
        public ISet<string> Enum { get; set; }

        [JsonProperty("elements")]
        [JsonPropertyName("elements")]
        public Schema Elements { get; set; }

        [JsonProperty("properties")]
        [JsonPropertyName("properties")]
        public IDictionary<string, Schema> Properties { get; set; }

        [JsonProperty("optionalProperties")]
        [JsonPropertyName("optionalProperties")]
        public IDictionary<string, Schema> OptionalProperties { get; set; }

        [JsonProperty("additionalProperties")]
        [JsonPropertyName("additionalProperties")]
        public bool? AdditionalProperties { get; set; }

        [JsonProperty("values")]
        [JsonPropertyName("values")]
        public Schema Values { get; set; }

        [JsonProperty("discriminator")]
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("mapping")]
        [JsonPropertyName("mapping")]
        public IDictionary<string, Schema> Mapping { get; set; }

        public void Verify()
        {
            verify(this);
        }

        public Form Form()
        {
            if (Ref != null)
            {
                return Jtd.Form.Ref;
            }

            if (Type != null)
            {
                return Jtd.Form.Type;
            }

            if (Enum != null)
            {
                return Jtd.Form.Enum;
            }

            if (Elements != null)
            {
                return Jtd.Form.Elements;
            }

            if (Properties != null || OptionalProperties != null)
            {
                return Jtd.Form.Properties;
            }

            if (Values != null)
            {
                return Jtd.Form.Values;
            }

            if (Discriminator != null)
            {
                return Jtd.Form.Discriminator;
            }

            return Jtd.Form.Empty;
        }

        // Index of valid form "signatures" -- i.e., combinations of the presence of the
        // keywords (in order):
        //
        // ref type enum elements properties optionalProperties additionalProperties
        // values discriminator mapping
        //
        // The keywords "definitions", "nullable", and "metadata" are not included here,
        // because they would restrict nothing.
        private static bool[][] VALID_FORMS = new bool[][] {
            // Empty form
            new bool[] { false, false, false, false, false, false, false, false, false, false },
            // Ref form
            new bool[]{ true, false, false, false, false, false, false, false, false, false },
            // Type form
            new bool[]{ false, true, false, false, false, false, false, false, false, false },
            // Enum form
            new bool[]{ false, false, true, false, false, false, false, false, false, false },
            // Elements form
            new bool[]{ false, false, false, true, false, false, false, false, false, false },
            // Properties form -- properties or optional properties or both, and never
            // additional properties on its own
            new bool[]{ false, false, false, false, true, false, false, false, false, false },
            new bool[]{ false, false, false, false, false, true, false, false, false, false },
            new bool[]{ false, false, false, false, true, true, false, false, false, false },
            new bool[]{ false, false, false, false, true, false, true, false, false, false },
            new bool[]{ false, false, false, false, false, true, true, false, false, false },
            new bool[]{ false, false, false, false, true, true, true, false, false, false },
            // Values form
            new bool[]{ false, false, false, false, false, false, false, true, false, false },
            // Discriminator form
            new bool[]{ false, false, false, false, false, false, false, false, true, true },
        };

        private void verify(Schema root)
        {
            bool[] formSignature = {
                Ref != null,
                Type != null,
                Enum != null,
                Elements != null,
                Properties != null,
                OptionalProperties != null,
                AdditionalProperties != null,
                Values != null,
                Discriminator != null,
                Mapping != null,
            };

            if (!Array.Exists(VALID_FORMS, sig => sig.SequenceEqual(formSignature)))
            {
                throw new InvalidSchemaException("invalid form");
            }

            if (Definitions != null)
            {
                if (this != root)
                {
                    throw new InvalidSchemaException("non-root definitions");
                }

                foreach (Schema schema in Definitions.Values)
                {
                    schema.verify(root);
                }
            }

            if (Ref != null)
            {
                if (root.Definitions == null || !root.Definitions.ContainsKey(Ref))
                {
                    throw new InvalidSchemaException("ref to non-existent definition");
                }
            }

            if (Enum != null)
            {
                if (Enum.Count == 0)
                {
                    throw new InvalidSchemaException("empty enum");
                }
            }

            if (Elements != null)
            {
                Elements.verify(root);
            }

            if (Properties != null)
            {
                foreach (Schema schema in Properties.Values)
                {
                    schema.verify(root);
                }
            }

            if (OptionalProperties != null)
            {
                foreach (Schema schema in OptionalProperties.Values)
                {
                    schema.verify(root);
                }
            }

            if (Properties != null && OptionalProperties != null)
            {
                foreach (string key in Properties.Keys)
                {
                    if (OptionalProperties.ContainsKey(key))
                    {
                        throw new InvalidSchemaException("properties shares key with optionalProperties");
                    }
                }
            }

            if (Values != null)
            {
                Values.verify(root);
            }

            if (Mapping != null)
            {
                foreach (Schema schema in Mapping.Values)
                {
                    schema.verify(root);

                    if (schema.Form() != Jtd.Form.Properties)
                    {
                        throw new InvalidSchemaException("mapping value not of properties form");
                    }

                    if (schema.Properties != null && schema.Properties.ContainsKey(Discriminator))
                    {
                        throw new InvalidSchemaException("discriminator shares keys with mapping properties");
                    }

                    if (schema.OptionalProperties != null && schema.OptionalProperties.ContainsKey(Discriminator))
                    {
                        throw new InvalidSchemaException("discriminator shares keys with mapping properties");
                    }

                    // Nullable is not "bool", but "bool?". Comparing to true is
                    // doing something useful here: it's excluding the
                    // possibility of Nullable being null.
                    if (schema.Nullable == true)
                    {
                        throw new InvalidSchemaException("mapping value is nullable");
                    }
                }
            }
        }
    }
}
