using System.Collections.Generic;

namespace Jtd.Jtd
{
    public interface IJson
    {
        bool IsNull();

        bool IsBoolean();

        bool IsNumber();

        bool IsString();

        bool IsArray();

        bool IsObject();

        bool AsBoolean();

        double AsNumber();

        string AsString();

        IList<IJson> AsArray();

        IDictionary<string, IJson> AsObject();
    }
}
