// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodTests.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Core.Tests
{
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// The extension method tests.
    /// </summary>
    public class ExtensionMethodTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("These are the lyrics")]
        [InlineData("These are the lyrics\rWith another line")]
        internal void TestUnchangedLyrics(string lyrics)
        {
            var mockTag = new Moq.Mock<TagLib.Tag>();
            mockTag.SetupGet(_ => _.Lyrics).Returns(lyrics);
            mockTag.Object.CleanLyrics().Should().BeFalse();
            mockTag.VerifyGet(_ => _.Lyrics);
            mockTag.VerifySet(_ => _.Lyrics = Moq.It.IsAny<string>(), Moq.Times.Never);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("   \r\nThese are the lyrics\r\n   ", "These are the lyrics")]
        [InlineData("   \rThese are the lyrics\r   ", "These are the lyrics")]
        [InlineData("These are the lyrics\r\nWith another line", "These are the lyrics\rWith another line")]
        [InlineData("These are the lyrics\rwith a line starting with lowercase", "These are the lyrics\rWith a line starting with lowercase")]
        [InlineData("These are the lyrics\r\nWith a trailing new line\r\n", "These are the lyrics\rWith a trailing new line")]
        internal void TestChangedLyrics(string input, string expected)
        {
            var mockTag = new Moq.Mock<TagLib.Tag>();
            mockTag.SetupGet(_ => _.Lyrics).Returns(input);
            mockTag.Object.CleanLyrics().Should().BeTrue();
            mockTag.VerifyGet(_ => _.Lyrics);
            mockTag.VerifySet(_ => _.Lyrics = expected, Moq.Times.Once);
        }
    }
}
