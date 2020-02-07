using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;

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

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);
            //var repo = new RepositorioFale();

            var handler = new CadastraTarefaHandler(repo);

            // act
            handler.Execute(comando);  // SUT >> CadastraTarefaHandlerExecute

            // assert
            //Assert.True(true);
            var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);
        }

        [Fact]
        public void QaundoExceptionForLancada_RessultadoDeveSerFalse()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo);

            // act
            CommandResult resultado = handler.Execute(comando);

            // assert
            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExceptForLancada_DeveLogarAMenssagemDaExcessao()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo);

            // act
            CommandResult resultado = handler.Execute(comando);

            // assert

        }
    }
}
