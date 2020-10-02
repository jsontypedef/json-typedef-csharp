using System.Collections.Generic;
using System;
using System.Globalization;

namespace Jtd.Jtd
{
    public class Validator
    {
        public int MaxDepth { get; set; }
        public int MaxErrors { get; set; }

        public IList<ValidationError> Validate(Schema schema, IJson instance)
        {
            ValidationState state = new ValidationState {
                Root = schema,
                InstanceTokens = new List<string>(),
                SchemaTokens = new List<List<string>>() { new List<string>() },
                Errors = new List<ValidationError>(),
                MaxErrors = MaxErrors,
            };

            try
            {
                validate(state, schema, instance);
            }
            catch (MaxErrorsReachedException)
            {
                // This is a dummy error. We should swallow this, and just
                // returned the abridged set of errors.
            }

            return state.Errors;
        }

        private void validate(ValidationState state, Schema schema, IJson instance, string parentTag = null)
        {
            if (schema.Nullable == true && instance.IsNull())
            {
                return;
            }

            switch (schema.Form())
            {
                case Form.Ref:
                    state.SchemaTokens.Add(new List<string>() { "definitions", schema.Ref });

                    if (state.SchemaTokens.Count == MaxDepth)
                    {
                        throw new MaxDepthExceededException();
                    }

                    validate(state, state.Root.Definitions[schema.Ref], instance);
                    state.SchemaTokens.RemoveAt(state.SchemaTokens.Count - 1);

                    break;
                case Form.Type:
                    state.PushSchemaToken("type");

                    switch (schema.Type)
                    {
                        case Type.Boolean:
                            if (!instance.IsBoolean())
                            {
                                state.PushError();
                            }

                            break;
                        case Type.Float32:
                        case Type.Float64:
                            if (!instance.IsNumber())
                            {
                                state.PushError();
                            }

                            break;
                        case Type.Int8:
                            validateInt(state, instance, -128, 127);
                            break;
                        case Type.Uint8:
                            validateInt(state, instance, 0, 255);
                            break;
                        case Type.Int16:
                            validateInt(state, instance, -32768, 32767);
                            break;
                        case Type.Uint16:
                            validateInt(state, instance, 0, 65535);
                            break;
                        case Type.Int32:
                            validateInt(state, instance, -2147483648, 2147483647);
                            break;
                        case Type.Uint32:
                            validateInt(state, instance, 0, 4294967295L);
                            break;
                        case Type.String:
                            if (!instance.IsString())
                            {
                                state.PushError();
                            }

                            break;
                        case Type.Timestamp:
                            if (instance.IsString())
                            {
                                try {
                                    DateTime.ParseExact(
                                        instance.AsString(),
                                        "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
                                        CultureInfo.InvariantCulture
                                    );
                                }
                                catch (FormatException)
                                {
                                    state.PushError();
                                }
                            }
                            else
                            {
                                state.PushError();
                            }

                            break;
                    }

                    state.PopSchemaToken();
                    break;
                case Form.Enum:
                    state.PushSchemaToken("enum");

                    if (!instance.IsString() || !schema.Enum.Contains(instance.AsString()))
                    {
                        state.PushError();
                    }

                    state.PopSchemaToken();
                    break;
                case Form.Elements:
                    state.PushSchemaToken("elements");

                    if (instance.IsArray())
                    {
                        IList<IJson> list = instance.AsArray();
                        for (int i = 0; i < list.Count; i++)
                        {
                            state.PushInstanceToken(i.ToString());
                            validate(state, schema.Elements, list[i]);
                            state.PopInstanceToken();
                        }
                    }
                    else
                    {
                        state.PushError();
                    }

                    state.PopSchemaToken();
                    break;
                case Form.Properties:
                    if (!instance.IsObject())
                    {
                        if (schema.Properties != null)
                        {
                            state.PushSchemaToken("properties");
                        }
                        else
                        {
                            state.PushSchemaToken("optionalProperties");
                        }

                        state.PushError();
                        state.PopSchemaToken();
                        return;
                    }

                    IDictionary<string, IJson> obj = instance.AsObject();

                    if (schema.Properties != null)
                    {
                        state.PushSchemaToken("properties");

                        foreach (var entry in schema.Properties)
                        {
                            state.PushSchemaToken(entry.Key);

                            if (obj.ContainsKey(entry.Key))
                            {
                                state.PushInstanceToken(entry.Key);
                                validate(state, entry.Value, obj[entry.Key]);
                                state.PopInstanceToken();
                            }
                            else
                            {
                                state.PushError();
                            }

                            state.PopSchemaToken();
                        }

                        state.PopSchemaToken();
                    }

                    if (schema.OptionalProperties != null)
                    {
                        state.PushSchemaToken("optionalProperties");

                        foreach (var entry in schema.OptionalProperties)
                        {
                            state.PushSchemaToken(entry.Key);

                            if (obj.ContainsKey(entry.Key))
                            {
                                state.PushInstanceToken(entry.Key);
                                validate(state, entry.Value, obj[entry.Key]);
                                state.PopInstanceToken();
                            }

                            state.PopSchemaToken();
                        }

                        state.PopSchemaToken();
                    }

                    if (schema.AdditionalProperties != true)
                    {
                        foreach (string key in obj.Keys)
                        {
                            if (schema.Properties != null && schema.Properties.ContainsKey(key))
                            {
                                continue;
                            }

                            if (schema.OptionalProperties != null && schema.OptionalProperties.ContainsKey(key))
                            {
                                continue;
                            }

                            if (key == parentTag)
                            {
                                continue;
                            }

                            state.PushInstanceToken(key);
                            state.PushError();
                            state.PopInstanceToken();
                        }
                    }

                    break;
                case Form.Values:
                    state.PushSchemaToken("values");

                    if (instance.IsObject())
                    {
                        foreach (var entry in instance.AsObject())
                        {
                            state.PushInstanceToken(entry.Key);
                            validate(state, schema.Values, entry.Value);
                            state.PopInstanceToken();
                        }
                    }
                    else
                    {
                        state.PushError();
                    }

                    state.PopSchemaToken();
                    break;
                case Form.Discriminator:
                    if (!instance.IsObject())
                    {
                        state.PushSchemaToken("discriminator");
                        state.PushError();
                        state.PopSchemaToken();
                        break;
                    }

                    obj = instance.AsObject();

                    if (!obj.ContainsKey(schema.Discriminator))
                    {
                        state.PushSchemaToken("discriminator");
                        state.PushError();
                        state.PopSchemaToken();
                        break;
                    }

                    if (!obj[schema.Discriminator].IsString())
                    {
                        state.PushSchemaToken("discriminator");
                        state.PushInstanceToken(schema.Discriminator);
                        state.PushError();
                        state.PopInstanceToken();
                        state.PopSchemaToken();
                        break;
                    }

                    if (!schema.Mapping.ContainsKey(obj[schema.Discriminator].AsString()))
                    {
                        state.PushSchemaToken("mapping");
                        state.PushInstanceToken(schema.Discriminator);
                        state.PushError();
                        state.PopInstanceToken();
                        state.PopSchemaToken();
                        break;
                    }

                    state.PushSchemaToken("mapping");
                    state.PushSchemaToken(obj[schema.Discriminator].AsString());
                    validate(
                        state,
                        schema.Mapping[obj[schema.Discriminator].AsString()],
                        instance,
                        schema.Discriminator
                    );
                    state.PopSchemaToken();
                    state.PopSchemaToken();

                    break;
            }
        }

        private void validateInt(ValidationState state, IJson instance, long min, long max)
        {
            if (!instance.IsNumber())
            {
                state.PushError();
                return;
            }

            double val = instance.AsNumber();
            if (val < min || val > max || val != Math.Round(val))
            {
                state.PushError();
            }
        }

        private class ValidationState
        {
            public Schema Root { get; set; }
            public List<string> InstanceTokens { get; set; }
            public List<List<string>> SchemaTokens { get; set; }
            public List<ValidationError> Errors { get; set; }
            public int MaxErrors { get; set; }

            public void PushInstanceToken(string token)
            {
                InstanceTokens.Add(token);
            }

            public void PopInstanceToken()
            {
                InstanceTokens.RemoveAt(InstanceTokens.Count - 1);
            }

            public void PushSchemaToken(string token)
            {
                SchemaTokens[SchemaTokens.Count - 1].Add(token);
            }

            public void PopSchemaToken()
            {
                IList<string> last = SchemaTokens[SchemaTokens.Count - 1];
                last.RemoveAt(last.Count - 1);
            }

            public void PushError()
            {
                Errors.Add(new ValidationError {
                    InstancePath = new List<string>(InstanceTokens),
                    SchemaPath = new List<string>(SchemaTokens[SchemaTokens.Count - 1]),
                });

                if (Errors.Count == MaxErrors)
                {
                    throw new MaxErrorsReachedException();
                }
            }
        }

        private class MaxErrorsReachedException : Exception
        {
            public MaxErrorsReachedException() : base()
            {
            }
        }
    }
}
