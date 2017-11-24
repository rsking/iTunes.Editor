// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodTests.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Core.Tests
{
    using FluentAssertions;
    using Xunit;

    internal class ExtensionMethodTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("These are the lyrics")]
        [InlineData("These are the lyrics\r\nWith another line")]
        private void TestUnchangedLyrics(string lyrics)
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
        [InlineData("These are the lyrics\rWith another line", "These are the lyrics\r\nWith another line")]
        [InlineData("These are the lyrics\r\nwith another line", "These are the lyrics\r\nWith another line")]
        [InlineData("These are the lyrics\rWith another line\r", "These are the lyrics\r\nWith another line")]
        private void TestChangedLyrics(string input, string expected)
        {
            var mockTag = new Moq.Mock<TagLib.Tag>();
            mockTag.SetupGet(_ => _.Lyrics).Returns(input);
            mockTag.Object.CleanLyrics().Should().BeTrue();
            mockTag.VerifyGet(_ => _.Lyrics);
            mockTag.VerifySet(_ => _.Lyrics = expected, Moq.Times.Once);
        }
    }
}
