using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;
using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class AutoCadastroHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<AutoCadastroCommand, ResultadoGenerico<long>>, ResultadoGenerico<long>>
    {
        
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        
        public async Task<ResultadoGenerico<long>> Handle(Command<AutoCadastroCommand, ResultadoGenerico<long>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                
                var usuarioExistente = await _unitOfWork.VerificarEMailCadastrado(data.EMail);

                if (usuarioExistente)
                {
                    return new ResultadoGenerico<long>(false, "Já existe um usuário com este e-mail", 0);
                }

                
                var usuarioResult = UsuarioEntidade.Criar(data.Nome, data.EMail, 
                    EncriptadorUtil.Criptografar(data.Senha), null);

                
                await _unitOfWork.InserirAsync(usuarioResult.Dados);

                
                await _unitOfWork.SaveChangesAsync();

                
                return new ResultadoGenerico<long>(true, "Usuário cadasrado com sucesso", usuarioResult.Dados.Id);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<long>(false, "Erro ao cadastrar usuário. Desc.:" + ex.Message, 0);
            }

        }
    }

    public class AutoCadastroCommand
    {
        public string Nome;
        public string EMail;
        public string Senha;
    }
}
