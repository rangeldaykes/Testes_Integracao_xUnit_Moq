using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using System;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaInfoValidas_DeveIncluirNoBD()
        {
            // arrange
            var comando = new CadastraTarefa(
                "Estudar Xunit",
                new Core.Models.Categoria("Estudo"),
                new DateTime(2019, 12, 31));

            var repo = new RepositorioFale();

            var handler = new CadastraTarefaHandler(repo);

            // act
            handler.Execute(comando);  // SUT >> CadastraTarefaHandlerExecute

            // assert
            //Assert.True(true);
            var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);

            // Criar Comando
            // executar o comando

        }
    }
}
