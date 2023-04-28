using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ATLManagerXUnit
{
	public class AlertaRecadoControllerTest
	{
		private readonly ATLManagerAuthContext _context;
		private readonly NotificacoesController _controller;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;
		private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

		public AlertaRecadoControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
			_roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

			_controller = new NotificacoesController(_context, _userManagerMock.Object, _roleManagerMock.Object);

		}

		// Teste unitário para o método Create
		[Fact]
		public async Task Create_ValidModel_Success()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png", AtlId = Guid.NewGuid() };


			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.Users.Add(user); // Adicione esta linha
			_context.ContaAdministrativa.Add(userAccount);
			_context.SaveChanges();

			var notificao = new Notificacao
			{
				NotificacaoId = Guid.NewGuid(),
				Titulo = "Noti teste",
				Mensagem = "Mensagem Teste",
				DataNotificacao = DateTime.Now,
				Lida = false,
				UserId = userAccount.UserId
			};

			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			var result = await _controller.Create(notificao);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(RecibosController.Index), redirectToActionResult.ActionName);
		}

		[Fact]
		public async void Edit_WithValidData_UpdatesNotificacao()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var notificacao = new Notificacao { NotificacaoId = Guid.NewGuid(), Titulo = "Noti Teste", Mensagem = "Mensagem Teste", Lida = false, UserId = userId, DataNotificacao = DateTime.Now };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Notificacoes.Add(notificacao);
			_context.SaveChanges();

			// Detach the original Notificacao entity from the DbContext
			_context.Entry(notificacao).State = EntityState.Detached;


			var notificacaoEdit = new Notificacao { NotificacaoId = notificacao.NotificacaoId, Titulo = "Noti editado", Mensagem = "Mensagem editada", Lida = true, UserId = userId, DataNotificacao = DateTime.Now };

			// Act
			var result = await _controller.Edit(notificacao.NotificacaoId, notificacaoEdit);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedNotificacao = await _context.Notificacoes.FindAsync(notificacao.NotificacaoId);
			Assert.Equal(notificacaoEdit.Titulo, updatedNotificacao.Titulo);
			Assert.Equal(notificacaoEdit.Mensagem, updatedNotificacao.Mensagem);
			Assert.Equal(notificacaoEdit.Lida, updatedNotificacao.Lida);
			Assert.Equal(notificacaoEdit.DataNotificacao, updatedNotificacao.DataNotificacao);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesNotificacao()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var notificacao = new Notificacao { NotificacaoId = Guid.NewGuid(), Titulo = "Noti Teste", Mensagem = "Mensagem Teste", Lida = false, UserId = userId, DataNotificacao = DateTime.Now };


			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Notificacoes.Add(notificacao);
			_context.SaveChanges();

			// Act
			int initialNotificacaoCount = await _context.Notificacoes.CountAsync();
			var result = await _controller.DeleteConfirmed(notificacao.NotificacaoId);
			int finalNotificacaoCount = await _context.Notificacoes.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialNotificacaoCount - 1, finalNotificacaoCount);
		}
	}
}
