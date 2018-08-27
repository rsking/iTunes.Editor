namespace ITunes.Editor.PList
{
    internal static class Resources
    {
        public static System.IO.Stream TestBin => typeof(Resources).Assembly.GetManifestResourceStream(typeof(Resources).Namespace + ".testBin.plist");

        public static System.IO.Stream TestXml => typeof(Resources).Assembly.GetManifestResourceStream(typeof(Resources).Namespace + ".testXml.plist");
    }
}
