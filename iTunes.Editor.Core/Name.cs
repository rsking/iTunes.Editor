// -----------------------------------------------------------------------
// <copyright file="Name.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

using Humanizer;

/// <summary>
/// The performer name.
/// </summary>
public readonly struct Name : IEquatable<Name>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> struct.
    /// </summary>
    /// <param name="first">The first name.</param>
    /// <param name="last">The last name.</param>
    public Name(string? first, string last) => (this.First, this.Last) = (first?.Transform(To.LowerCase).Transform(To.TitleCase), last.Transform(To.LowerCase).Transform(To.TitleCase));

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public string? First { get; }

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public string Last { get; }

    /// <summary>
    /// The equals operator.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Name left, Name right) => left.Equals(right);

    /// <summary>
    /// The not-equals operator.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Name left, Name right) => !(left == right);

    /// <summary>
    /// Creates a <see cref="Name"/> from an inversed name such as 'WALTERS D'.
    /// </summary>
    /// <param name="name">The inversed name.</param>
    /// <returns>The output name.</returns>
    public static Name FromInversedName(string name)
    {
        if (name is null)
        {
            return new Name(default, string.Empty);
        }

        var split = name.Split(' ');
        var last = split[0];
        string? first = default;
        if (split.Length > 1)
        {
            first = split[1];
        }

        return new Name(first, last);
    }

    /// <summary>
    /// Creates a <see cref="Name"/> from a name such as 'D. WALTERS'.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The output name.</returns>
    public static Name FromName(string name)
    {
        if (name is null)
        {
            return new Name(string.Empty, string.Empty);
        }

        var split = name.Split(' ');
        string? first = default;
        string last;
        if (split.Length == 1)
        {
            last = split[0].Trim();
        }
        else
        {
            last = split[1].Trim();
            first = split[0].Trim('.');
        }

        return new Name(first, last);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(this.First))
        {
            return this.Last;
        }

        var joiner = this.First?.Length == 1 ? ". " : " ";
        return string.Concat(this.First, joiner, this.Last);
    }

    /// <inheritdoc/>
    public bool Equals(Name other) => string.Equals(this.First, other.First, StringComparison.Ordinal)
        && string.Equals(this.Last, other.Last, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Name name ? this.Equals(name) : base.Equals(obj);

    /// <inheritdoc/>
    public override int GetHashCode() => (this.First, this.Last).GetHashCode();
}
