namespace Jtd.Jtd
{
    /// <summary>
    /// Enum <c>Form</c> represents a JSON Type Definition schema form.
    /// </summary>
    ///
    /// <remarks>
    /// All correct JSON Type Definition schemas take on a particular "form",
    /// which is essentially a particular combination of keywords. Each member
    /// of <c>Form</c> represents one of the forms a schema may take on.
    /// </remarks>
    ///
    /// <seealso cref="Schema.Form" />
    public enum Form
    {
        /// <summary>
        /// The empty form. Only the keywords shared by all schema forms
        /// (<c>metadata</c> and <c>nullable</c>) may be used.
        /// </summary>
        Empty,

        /// <summary>
        /// The ref form. The <c>ref</c> keyword is used.
        /// </summary>
        Ref,

        /// <summary>
        /// The type form. The <c>type</c> keyword is used.
        /// </summary>
        Type,

        /// <summary>
        /// The enum form. The <c>enum</c> keyword is used.
        /// </summary>
        Enum,

        /// <summary>
        /// The elements form. The <c>elements</c> keyword is used.
        /// </summary>
        Elements,

        /// <summary>
        /// The properties form. The <c>additionalProperties</c> keyword, and
        /// one or both of <c>properties</c> and <c>optionalProperties</c> are
        /// used.
        /// </summary>
        Properties,

        /// <summary>
        /// The values form. The <c>values</c> keyword is used.
        /// </summary>
        Values,

        /// <summary>
        /// The discriminator form. The <c>discriminator</c> and <c>mapping</c>
        /// keywords are used.
        /// </summary>
        Discriminator,
    }
}
