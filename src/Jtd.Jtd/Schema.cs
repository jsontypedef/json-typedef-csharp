using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Jtd.Jtd
{
    /// <summary>
    /// Class <c>Schema</c> represents a JSON Type Definition schema.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This class is designed to be usable as a serializable (and
    /// deserializable) class for use with <see
    /// cref="T:Newtonsoft.Json.JsonConvert" /> or <see
    /// cref="T:System.Text.Json.JsonSerializer" />.
    /// </para>
    ///
    /// <para>
    /// The correctness of a JSON Type Definition schema cannot be expressed
    /// entirely within CSharp's type system. To that end, consider using <see
    /// cref="Verify" /> to ensure that a <c>Schema</c> represents a correct
    /// JSON Type Definition schema.
    /// </para>
    /// </remarks>
    public class Schema
    {
        /// <value>
        /// Corresponds to the <c>metadata</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("metadata")]
        [JsonPropertyName("metadata")]
        public IDictionary<string, object> Metadata { get; set; }

        /// <value>
        /// Corresponds to the <c>nullable</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("nullable")]
        [JsonPropertyName("nullable")]
        public bool? Nullable { get; set; }

        /// <value>
        /// Corresponds to the <c>definitions</c> keyword in JSON Type
        /// Definition.
        /// </value>
        [JsonProperty("definitions")]
        [JsonPropertyName("definitions")]
        public IDictionary<string, Schema> Definitions { get; set; }

        /// <value>
        /// Corresponds to the <c>ref</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("ref")]
        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        /// <value>
        /// Corresponds to the <c>type</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public Type? Type { get; set; }

        /// <value>
        /// Corresponds to the <c>enum</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("enum")]
        [JsonPropertyName("enum")]
        public ISet<string> Enum { get; set; }

        /// <value>
        /// Corresponds to the <c>elements</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("elements")]
        [JsonPropertyName("elements")]
        public Schema Elements { get; set; }

        /// <value>
        /// Corresponds to the <c>properties</c> keyword in JSON Type
        /// Definition.
        /// </value>
        [JsonProperty("properties")]
        [JsonPropertyName("properties")]
        public IDictionary<string, Schema> Properties { get; set; }

        /// <value>
        /// Corresponds to the <c>optionalProperties</c> keyword in JSON Type
        /// Definition.
        /// </value>
        [JsonProperty("optionalProperties")]
        [JsonPropertyName("optionalProperties")]
        public IDictionary<string, Schema> OptionalProperties { get; set; }

        /// <value>
        /// Corresponds to the <c>additionalProperties</c> keyword in JSON Type
        /// Definition.
        /// </value>
        [JsonProperty("additionalProperties")]
        [JsonPropertyName("additionalProperties")]
        public bool? AdditionalProperties { get; set; }

        /// <value>
        /// Corresponds to the <c>values</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("values")]
        [JsonPropertyName("values")]
        public Schema Values { get; set; }

        /// <value>
        /// Corresponds to the <c>discriminator</c> keyword in JSON Type
        /// Definition.
        /// </value>
        [JsonProperty("discriminator")]
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        /// <value>
        /// Corresponds to the <c>mapping</c> keyword in JSON Type Definition.
        /// </value>
        [JsonProperty("mapping")]
        [JsonPropertyName("mapping")]
        public IDictionary<string, Schema> Mapping { get; set; }

        /// <summary>
        /// Verifies that the <c>Schema</c> represents a correct JSON Type
        /// Definition schema.
        /// </summary>
        ///
        /// <remarks>
        /// The JSON Type Definition specification has certain requirements on
        /// correct schemas that cannot be represented in CSharp's type system,
        /// such as requiring that all <c>ref</c> keywords have a corresponding
        /// definition. This method will check each of these requirements.
        /// </remarks>
        ///
        /// <exception cref="InvalidSchemaException">
        /// If the schema is not correct.
        /// </exception>
        public void Verify()
        {
            verify(this);
        }

        /// <summary>
        /// Determines the form the <c>Schema</c> takes on.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The JSON Type Definition specification requires that all schemas
        /// take on a particular "form", which is essentially a particular
        /// combination of keywords. This method determines which form a schema
        /// is using.
        /// </para>
        ///
        /// <para>
        /// The return value of this method is meaningful only if the schema is,
        /// or is a sub-schema of, a correct schema. In other words, calling
        /// <see cref="Verify" /> on this schema's root schema must not have
        /// raised an exception.
        /// </para>
        /// </remarks>
        ///
        /// <returns>
        /// The form the schema takes on, assuming it is a correct schema.
        /// </returns>
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
