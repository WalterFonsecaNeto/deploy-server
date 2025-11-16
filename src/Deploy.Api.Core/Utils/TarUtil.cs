using ICSharpCode.SharpZipLib.Tar;

namespace Deploy.Api.Core.Utils
{
    public static class TarUtil
    {
        public static Stream CreateTarFromDirectory(string directory)
        {
            var outStream = new MemoryStream();

            using (var tarArchive = TarArchive.CreateOutputTarArchive(outStream))
            {
                tarArchive.IsStreamOwner = false; // não fecha MemoryStream
                AddFiles(tarArchive, directory, "");
            }

            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        private static void AddFiles(TarArchive tar, string sourceDir, string parentPath)
        {
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var entry = TarEntry.CreateEntryFromFile(file);

                entry.Name = Path.Combine(parentPath, Path.GetFileName(file))
                    .Replace("\\", "/");

                tar.WriteEntry(entry, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.Combine(parentPath, Path.GetFileName(dir)).Replace("\\", "/");
                AddFiles(tar, dir, dirName);
            }
        }
    }
}
