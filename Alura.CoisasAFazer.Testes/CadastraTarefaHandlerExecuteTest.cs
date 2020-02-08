using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecuteTest
    {
        [Fact]
        public void DadaTarefaInfoValidas_DeveIncluirNoBD()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);
            //var repo = new RepositorioFale();

            var handler = new CadastraTarefaHandler(repo, mock.Object);

            // act
            handler.Execute(comando);  // SUT >> CadastraTarefaHandlerExecute

            // assert
            //Assert.True(true);
            var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);
        }

        delegate void CapturaMensagemLog(
            LogLevel level,
            EventId eventId,
            object state,
            Exception exception,
            Func<object, Exception, string> function);

        [Fact]
        public void DadaTarefaInfoValidas_DeveLogar()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            LogLevel levelCapturado = LogLevel.Error;

            CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
            {
                levelCapturado = level;
            };

            mockLogger.Setup(l =>
                l.Log(
                    It.IsAny<LogLevel>(),      // nivel de log
                    It.IsAny<EventId>(), // identificador do evento 
                    It.Is<It.IsAnyType>((v, t) => true),  // objeto que será logado
                    It.IsAny<Exception>(),   // exceção que sera logada
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)) // função que converte objeto + exceção em string
            ).Callback(captura);

            var mock = new Mock<IRepositorioTarefas>();

            var handler = new CadastraTarefaHandler(mock.Object, mockLogger.Object);

            // act
            handler.Execute(comando);  // SUT >> CadastraTarefaHandlerExecute

            // assert
            Assert.Equal(LogLevel.Debug, levelCapturado);
        }

        [Fact]
        public void QaundoExceptionForLancada_RessultadoDeveSerFalse()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            // act
            CommandResult resultado = handler.Execute(comando);

            // assert
            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExceptForLancada_DeveLogarAMenssagemDaExcessao()
        {
            // arrange
            var menssagemDeErroEsperada = "Houve um erro na inclusão de tarefas";
            var excessaoEsperada = new Exception(menssagemDeErroEsperada);

            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(excessaoEsperada);

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            // act
            CommandResult resultado = handler.Execute(comando);

            // assert
            // l.LogError is a extension method, extensions methods are not suported for lib moq
            //mockLogger.Verify(l => l.LogError(menssagemDeErroEsperada), Times.Once);
            mockLogger.Verify(l =>
                l.Log(
                    LogLevel.Error,      // nivel de log
                    It.IsAny<EventId>(), // identificador do evento 
                    It.Is<It.IsAnyType>((v, t) => true),  // objeto que será logado
                    excessaoEsperada,    // exceção que sera logada
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), // função que converte objeto + exceção em string
                Times.Once());
        }
    }
}
