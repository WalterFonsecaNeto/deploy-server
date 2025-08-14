using System.IO.Compression;

namespace Deploy.Api.Core.Utils
{
    public static class UnzipUtil
    {
        public static async Task<string> ExtrairZipAsync(string caminhoZip, string destino)
        {
            Directory.CreateDirectory(destino);

            Console.WriteLine($"[UnzipUtil] Extraindo zip para: {destino}");

            ZipFile.ExtractToDirectory(caminhoZip, destino, true);

            var itens = Directory.GetFileSystemEntries(destino);

            // Caso tenha apenas uma pasta dentro, entrar nela
            if (itens.Length == 1 && Directory.Exists(itens[0]))
            {
                Console.WriteLine($"📂 Detectada pasta interna: {itens[0]}");
                return itens[0];
            }

            return destino;
        }
    }
}
