namespace ITunes.Editor.PList
{
    using FluentAssertions;
    using System;
    using System.Text;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="PList"/>.
    /// </summary>
    public class PListTest
    {
        private readonly PList plist;

        /// <summary>
        /// Initialises a new instance of the <see cref="PListTest"/> class.
        /// </summary>
        public PListTest()
        {
            var data = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple Computer//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>Major Version</key><integer>1</integer>
	<key>Minor Version</key><integer>1</integer>
	<key>Application Version</key><string>12.7.5.9</string>
	<key>Date</key><date>2018-06-19T01:16:09Z</date>
	<key>Features</key><integer>5</integer>
	<key>Show Content Ratings</key><true/>
	<key>Library Persistent ID</key><string>A127D4A525BA0F0E</string>
	<key>Tracks</key>
	<dict>
		<key>2934</key>
		<dict>
			<key>Track ID</key><integer>2934</integer>
			<key>Size</key><integer>5858125</integer>
			<key>Total Time</key><integer>172960</integer>
			<key>Disc Number</key><integer>1</integer>
			<key>Disc Count</key><integer>1</integer>
			<key>Track Number</key><integer>14</integer>
			<key>Track Count</key><integer>17</integer>
			<key>Year</key><integer>2002</integer>
			<key>BPM</key><integer>130</integer>
			<key>Date Modified</key><date>2015-06-16T07:44:22Z</date>
			<key>Date Added</key><date>2016-10-18T03:44:51Z</date>
			<key>Bit Rate</key><integer>256</integer>
			<key>Sample Rate</key><integer>44100</integer>
			<key>Release Date</key><date>2002-06-04T12:00:00Z</date>
			<key>Rating</key><integer>100</integer>
			<key>Artwork Count</key><integer>1</integer>
			<key>Persistent ID</key><string>E2D04F524C84ECEC</string>
			<key>Clean</key><true/>
			<key>Track Type</key><string>File</string>
			<key>Purchased</key><true/>
			<key>File Folder Count</key><integer>5</integer>
			<key>Library Folder Count</key><integer>1</integer>
			<key>Name</key><string>Goodbye</string>
			<key>Artist</key><string>Anna Waronker</string>
			<key>Album Artist</key><string>Anna Waronker</string>
			<key>Composer</key><string>Anna Waronker</string>
			<key>Album</key><string>Anna</string>
			<key>Genre</key><string>Alternative</string>
			<key>Kind</key><string>Purchased AAC audio file</string>
			<key>Sort Name</key><string>Goodbye</string>
			<key>Sort Album</key><string>Anna</string>
			<key>Sort Artist</key><string>Waronker, Anna</string>
			<key>Location</key><string>file://localhost/D:/Users/rskin/OneDrive/Music/iTunes/iTunes%20Media/Music/Anna%20Waronker/Anna/14%20Goodbye.m4a</string>
		</dict>
    </dict>
</dict>
</plist>";

            using (var stream = new System.IO.MemoryStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    writer.Write(data);
                }

                stream.Position = 0;
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PList));
                plist = serializer.Deserialize(stream) as PList;
            }
        }

        [Fact]
        private void TestVersion() => this.plist.Version.Should().Be(new Version(1, 0));

        [Fact]
        private void TestCount() => this.plist.Count.Should().Be(8);

        [Fact]
        private void TestIsReadOnly() => this.plist.IsReadOnly.Should().BeFalse();
    }
}
