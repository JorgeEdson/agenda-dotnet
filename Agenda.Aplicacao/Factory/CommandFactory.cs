using MediatR;

namespace Agenda.Aplicacao.Factory
{
    public class CommandFactory
    {
        public static Command<T, R> Create<T, R>(T data) => new Command<T, R>(data);

        // Command da "Fabrica" de comandos
        public class Command<T, R> : IRequest<R>
        {
            public T Data { get; set; }

            public Command(T data)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }
    }
}
