using Agenda.Dominio.Entidades;
using System.Text.Json;

namespace Agenda.Dominio.Helpers
{
    public static class GerenciadorArquivosHelper
    {
        private static string _raiz = AppDomain.CurrentDomain.BaseDirectory;

        public static string MostrarRaizAtual()
        {
            return _raiz;
        }

        public static void DefinirRaiz(string novaRaiz)
        {
            if (!Directory.Exists(novaRaiz))
            {
                Directory.CreateDirectory(novaRaiz);
            }
            _raiz = novaRaiz;
        }

        public static void CriarArquivo<T>(T entidade) where T : EntidadeBase
        {
            string nomeArquivo = $"{entidade.Id} - {typeof(T).Name}.txt";
            string caminhoArquivo = Path.Combine(_raiz, nomeArquivo);
            string json = JsonSerializer.Serialize(entidade, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(caminhoArquivo, json);
        }

        public static void ExcluirArquivoPorId(long id)
        {
            var arquivos = Directory.GetFiles(_raiz, $"{id} - *.txt");

            if (arquivos.Length == 0)
            {
                throw new FileNotFoundException("Nenhum arquivo encontrado com o ID especificado.");
            }

            foreach (var arquivo in arquivos)
            {
                File.Delete(arquivo);
            }
        }

        public static List<string> ListarArquivosNaRaiz()
        {
            var arquivos = Directory.GetFiles(_raiz, "*.txt");
            var nomesArquivos = new List<string>();

            foreach (var arquivo in arquivos)
            {
                nomesArquivos.Add(Path.GetFileName(arquivo));
            }

            return nomesArquivos;
        }
        
    }
}
