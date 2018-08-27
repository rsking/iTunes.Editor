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
        /// Initialises a new instance of the <see cref="XmlPListTests"/> class.
        /// </summary>
        public XmlPListTests()
        {
            using (var stream = Resources.TestXml)
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                plist = serializer.Deserialize(stream) as PList;
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
