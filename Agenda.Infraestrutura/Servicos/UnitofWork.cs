using Agenda.Dominio.Entidades;
using Agenda.Dominio.Interfaces;
using Agenda.Infraestrutura.Contextos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Infraestrutura.Servicos
{
    public class UnitofWork(Contexto contexto) : IUnitOfWork
    {
        private readonly Contexto _contexto = contexto;

        public async Task AtualizarAsync<T>(T entidade) where T : EntidadeBase
        {
            _contexto.Set<T>().Update(entidade);
        }

        public async Task<int> CountAsync<T>() where T : EntidadeBase
        {   
            return await _contexto.Set<T>().CountAsync();   
        }

        public async Task InserirAsync<T>(T entidade) where T : EntidadeBase
        {
            await _contexto.Set<T>().AddAsync(entidade);
        }

        public async Task<T?> ObterPorIdAsync<T>(long id) where T : EntidadeBase
        {   
            return await _contexto.Set<T>().FirstOrDefaultAsync(x=> x.Id == id);
        }
       
        public async Task<IEnumerable<T>> ObterTodosAsync<T>() where T : EntidadeBase
        {   
            return await _contexto.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterTodosPaginadoAsync<T>(int pagina, int linhasPorPagina) where T : EntidadeBase
        {   
            return await _contexto.Set<T>()
                .AsNoTracking()
                .Skip(pagina * linhasPorPagina)
                .Take(linhasPorPagina)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosComFiltroAsync(string? nome, string? email, bool? ativo, bool? administrador, int pagina, int linhasPorPagina)
        {
            var query = _contexto.Usuarios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(u => EF.Functions.Like(u.Nome, $"%{nome.Trim()}%"));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => EF.Functions.Like(u.Email, $"%{email.Trim()}%"));

            if (ativo.HasValue)
                query = query.Where(u => u.Ativo == ativo.Value);

            if (administrador.HasValue)
                query = query.Where(u => u.Administrador == administrador.Value);

            return await query
                .Skip(pagina * linhasPorPagina)
                .Take(linhasPorPagina)
                .ToListAsync();
        }

        public async Task RemoverAsync<T>(T entidade) where T : EntidadeBase
        {
            _contexto.Set<T>().Remove(entidade);
        }

        public Task RemoverLoteAsync<T>(IEnumerable<T> lote) where T : EntidadeBase
        {
            // Remove um lote de registros
            _contexto.RemoveRange(lote);

            return Task.CompletedTask;
        }

        public async Task<bool> VerificarEMailCadastrado(string email)
        {
            // AsNoTracking = ignorando alterações da tabela
            // FirstorDefault = Achar o primeiro encontrado. <> null = achou algo
            return await _contexto.Usuarios.AsNoTracking().Where(x => x.Email == email).FirstOrDefaultAsync() != null;
        }

        public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
        {
            return await _contexto.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _contexto.SaveChangesAsync(cancellationToken);
        }

        public async Task<Usuario?> ObterUsuarioComEnderecos(long id)
        {
            return await _contexto.Usuarios
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
