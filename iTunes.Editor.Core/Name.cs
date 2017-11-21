// -----------------------------------------------------------------------
// <copyright file="Name.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The performer name.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Name"/> class.
        /// </summary>
        /// <param name="first">The first name.</param>
        /// <param name="last">The last name.</param>
        public Name(string first, string last)
        {
            this.First = string.IsNullOrEmpty(first) ? first : string.Concat(first.Substring(0, 1).ToUpperInvariant(), first.Substring(1).ToLowerInvariant());
            this.Last = string.IsNullOrEmpty(last) ? last : string.Concat(last.Substring(0, 1).ToUpperInvariant(), last.Substring(1).ToLowerInvariant());
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string First { get; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string Last { get; }

        /// <summary>
        /// Creates a <see cref="Name"/> from an inversed name such as 'WALTERS D'
        /// </summary>
        /// <param name="name">The inversed name.</param>
        /// <returns>The output name.</returns>
        public static Name FromInversedName(string name)
        {
            var split = name.Split(' ');
            var last = split[0];
            string first = null;
            if (split.Length > 1)
            {
                first = split[1];
            }

            return new Name(first, last);
        }

        /// <summary>
        /// Creates a <see cref="Name"/> from a name such as 'D. WALTERS'
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The output name.</returns>
        public static Name FromName(string name)
        {
            var split = name.Split(' ');
            string last = null;
            string first = null;
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
            return string.IsNullOrEmpty(this.First)
                ? this.Last
                : string.Concat(this.First, this.First.Length == 1 ? ". " : " ", this.Last);
        }
    }
}
