using Agenda.Dominio.Entidades;
using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Interfaces
{
    public interface IUnitOfWork
    {
        #region Metodos Genericos
        Task InserirAsync<T>(T entidade) where T : EntidadeBase;
        Task AtualizarAsync<T>(T entidade) where T : EntidadeBase;
        Task RemoverAsync<T>(T entidade) where T : EntidadeBase;
        Task<T?> ObterPorIdAsync<T>(long id) where T : EntidadeBase;
        Task<IEnumerable<T>> ObterTodosAsync<T>() where T : EntidadeBase;
        Task<IEnumerable<T>> ObterTodosPaginadoAsync<T>(int pagina, int linhasPorPagina) where T : EntidadeBase;
        Task RemoverLoteAsync<T>(IEnumerable<T> lote) where T : EntidadeBase;
        Task<int> CountAsync<T>() where T : EntidadeBase;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        #endregion

        Task<IEnumerable<Usuario>> ObterUsuariosComFiltroAsync(
            string? nome,
            string? email,
            bool? ativo,
            bool? administrador,
            int pagina,
            int linhasPorPagina);

        Task<bool> VerificarEMailCadastrado(string email);

        Task<Usuario?> ObterUsuarioPorEmailAsync(string email);

        Task<Usuario?> ObterUsuarioComEnderecos(long id);
        
    }
}
