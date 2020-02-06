﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;

namespace Alura.CoisasAFazer.Testes
{    
    public class ObtemCategoriaPorIdExecuteTest
    {
        [Fact]
        public void QuandoIdForExistente_DeveChamarObtemCategoriaPorIdUmaUnicaVez() 
        {
            // arrange
            var idCategoria = 20;
            var comando = new ObtemCategoriaPorId(idCategoria);
            var mock = new Mock<IRepositorioTarefas>();
            var repo = mock.Object;
            var handler = new ObtemCategoriaPorIdHandler(repo);

            // act
            handler.Execute(comando);

            // assert
            mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once);
        }
    }
}
