using System.IO.Compression;

namespace Deploy.Api.Core.Utils
{
    public static class UnzipUtil
    {
        public static async Task<string?> ExtrairZipAsync(string caminhoZip, string destino)
        {
            try
            {
                Console.WriteLine($"[UnzipUtil] Processo Iniciado");

                Directory.CreateDirectory(destino);

                Console.WriteLine($"Extraindo zip para: {destino}");

                ZipFile.ExtractToDirectory(caminhoZip, destino, true);

                var itens = Directory.GetFileSystemEntries(destino);

                // Caso tenha apenas uma pasta dentro, entrar nela
                if (itens.Length == 1 && Directory.Exists(itens[0]))
                {
                    Console.WriteLine($"Detectada pasta interna: {itens[0]}");
                    return itens[0];
                }

                Console.WriteLine($"[UnzipUtil] Processo Finalizado");

                return destino;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[UnzipUtil] Erro ao extrair: {ex.Message}");
                return null; 
            }
        }
    }

}
