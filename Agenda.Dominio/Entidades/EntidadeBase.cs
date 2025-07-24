using Agenda.Dominio.Helpers;

namespace Agenda.Dominio.Entidades
{
    public abstract class EntidadeBase
    {
        public long Id { get; private set; }

        protected EntidadeBase()
        {
            Id = GeradorIdHelper.ProximoId();  // Força a geração de um novo ID
        }

        public void SetId(long id)
        {
            Id = id;
        }
    }
}
