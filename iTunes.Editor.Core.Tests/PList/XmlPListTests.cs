// -----------------------------------------------------------------------
// <copyright file="XmlPListTests.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="PList"/>.
    /// </summary>
    public class XmlPListTests
    {
        private readonly PList plist;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPListTests"/> class.
        /// </summary>
        public XmlPListTests()
        {
            using var stream = Resources.TestXml;
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
            this.plist = (PList)serializer.Deserialize(stream);
        }

        [Fact]
        internal void TestVersion() => this.plist.Version.Should().Be(new Version(1, 0));

        [Fact]
        internal void TestCount() => this.plist.Count.Should().Be(11);

        [Fact]
        internal void TestIsReadOnly() => this.plist.IsReadOnly.Should().BeFalse();
    }
}
