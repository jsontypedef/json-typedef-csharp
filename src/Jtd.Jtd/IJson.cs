using System.Collections.Generic;

namespace Jtd.Jtd
{
    /// <summary>
    /// Interface <c>IJson</c> represents the JSON data model.
    /// </summary>
    ///
    /// <remarks>
    /// In the CSharp ecosystem, the two primary JSON implementations,
    /// <c>Newtonsoft.Json</c> and <c>System.Text.Json</c>, have mutually
    /// incompatible representations of arbitrary JSON data. This interface
    /// exists to unify these data models, and to support other JSON
    /// implementations.
    /// </remarks>
    ///
    /// <seealso cref="NewtonsoftAdapter" />
    /// <seealso cref="SystemTextAdapter" />
    public interface IJson
    {
        /// <summary>
        /// Whether the data represents JSON <c>null</c>.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents JSON <c>null</c>, <c>false</c>
        /// otherwise.
        /// </returns>
        bool IsNull();

        /// <summary>
        /// Whether the data represents JSON <c>true</c> or <c>false</c>.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents JSON <c>true</c> or <c>false</c>,
        /// <c>false</c> otherwise.
        /// </returns>
        bool IsBoolean();

        /// <summary>
        /// Whether the data represents a JSON number. This includes "integers"
        /// (e.g. <c>3</c>), "floating-point numbers" (e.g. <c>3.14</c>), and
        /// "exponential numbers" (e.g. <c>3.14e5</c>).
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents a JSON number, <c>false</c>
        /// otherwise.
        /// </returns>
        bool IsNumber();

        /// <summary>
        /// Whether the data represents a JSON string. This includes strings
        /// that happen to represent timestamps, UUIDs, GUIDs, URLs, or other
        /// data commonly represented as strings in JSON.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents a JSON string, <c>false</c>
        /// otherwise.
        /// </returns>
        bool IsString();

        /// <summary>
        /// Whether the data represents a JSON array.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents a JSON array, <c>false</c>
        /// otherwise.
        /// </returns>
        bool IsArray();

        /// <summary>
        /// Whether the data represents a JSON object.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if the data represents a JSON object, <c>false</c>
        /// otherwise.
        /// </returns>
        bool IsObject();

        /// <summary>
        /// The boolean value represented by the JSON value.
        /// </summary>
        ///
        /// <remarks>
        /// If <see cref="IsBoolean" /> did not return <c>true</c>, then the
        /// behavior of this method is not well-defined.
        /// </remarks>
        ///
        /// <returns>
        /// The <c>bool</c> this data represents, assuming it represents a
        /// boolean value.
        /// </returns>
        bool AsBoolean();

        /// <summary>
        /// The numeric value represented by the JSON value.
        /// </summary>
        ///
        /// <remarks>
        /// If <see cref="IsNumber" /> did not return <c>true</c>, then the
        /// behavior of this method is not well-defined.
        /// </remarks>
        ///
        /// <returns>
        /// The <c>double</c> this data represents, assuming it represents a
        /// numeric value.
        /// </returns>
        double AsNumber();

        /// <summary>
        /// The string value represented by the JSON value.
        /// </summary>
        ///
        /// <remarks>
        /// If <see cref="IsString" /> did not return <c>true</c>, then the
        /// behavior of this method is not well-defined.
        /// </remarks>
        ///
        /// <returns>
        /// The <c>string</c> this data represents, assuming it represents a
        /// string value.
        /// </returns>
        string AsString();

        /// <summary>
        /// The array value represented by the JSON value.
        /// </summary>
        ///
        /// <remarks>
        /// If <see cref="IsArray" /> did not return <c>true</c>, then the
        /// behavior of this method is not well-defined.
        /// </remarks>
        ///
        /// <returns>
        /// The <c>IList{IJson}</c> this data represents, assuming it represents
        /// an array value.
        /// </returns>
        IList<IJson> AsArray();

        /// <summary>
        /// The object value represented by the JSON value.
        /// </summary>
        ///
        /// <remarks>
        /// If <see cref="IsObject" /> did not return <c>true</c>, then the
        /// behavior of this method is not well-defined.
        /// </remarks>
        ///
        /// <returns>
        /// The <c>IDictionary{string, IJson}</c> this data represents, assuming
        /// it represents an object value.
        /// </returns>
        IDictionary<string, IJson> AsObject();
    }
}
