namespace ITunes.Editor.PList
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class BinaryPListTests
    {
        private readonly PList plist;

        /// <summary>
        /// Initialises a new instance of the <see cref="XmlPListTests"/> class.
        /// </summary>
        public BinaryPListTests()
        {
            using (var stream = Resources.TestBin)
            {
                var formatter = new PListBinaryFormatter();
                plist = formatter.Deserialize(stream) as PList;
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
