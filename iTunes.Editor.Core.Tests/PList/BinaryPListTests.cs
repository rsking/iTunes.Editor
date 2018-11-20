// -----------------------------------------------------------------------
// <copyright file="BinaryPListTests.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="PListBinaryFormatter"/>.
    /// </summary>
    public class BinaryPListTests
    {
        private readonly PList plist;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPListTests"/> class.
        /// </summary>
        public BinaryPListTests()
        {
            using (var stream = Resources.TestBin)
            {
                var formatter = new PListBinaryFormatter();
                this.plist = formatter.Deserialize(stream) as PList;
            }
        }

        [Fact]
        private void TestVersion() => this.plist.Version.Should().Be(new Version(1, 0));

        [Fact]
        private void TestCount() => this.plist.Count.Should().Be(11);

        [Fact]
        private void TestIsReadOnly() => this.plist.IsReadOnly.Should().BeFalse();
    }
}
