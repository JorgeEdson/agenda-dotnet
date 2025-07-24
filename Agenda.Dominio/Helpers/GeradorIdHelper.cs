namespace Agenda.Dominio.Helpers
{
    public static class GeradorIdHelper
    {
        private static long _ultimoTimestamp = -1L;
        private static int _contador = 0;
        private static readonly object _bloqueio = new();

        public static long ProximoId()
        {
            try
            {
                lock (_bloqueio)
                {
                    var timestamp = ObterTimestampAtual();

                    if (timestamp == _ultimoTimestamp)
                    {
                        _contador++;
                    }
                    else
                    {
                        _contador = 0;
                        _ultimoTimestamp = timestamp;
                    }

                    return long.Parse($"{timestamp}{_contador:D4}");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o próximo ID.", ex);
            }
        }

        private static long ObterTimestampAtual()
        {
            return long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        }
    }
}
