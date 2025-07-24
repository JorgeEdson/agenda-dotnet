namespace Agenda.Dominio.Utils
{
    public class ResultadoPaginado<T>
    {        
        public int Total { get; set; }
        public T[] Dados { get; set; }

        public ResultadoPaginado(T[] dados, int total)
        {
            Dados = dados;
            Total = total;
        }
    }
}
