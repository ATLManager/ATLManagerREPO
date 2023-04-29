using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ATLManagerXUnit
{
	public class CalendarioAtividadeControllerTest
	{
		private readonly AtividadesController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public CalendarioAtividadeControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_controller = new AtividadesController(_context, _userManagerMock.Object, null);

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

			var viewModel = new AtividadeCreateViewModel
			{
				Name = "Atividade test",
				Description = "Descricao Test",
				StartDate = DateTime.Now.AddDays(1),
				EndDate = DateTime.Now.AddDays(3)
			};


			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			int initialAtividadeCount = await _context.Atividade.CountAsync();
			var result = await _controller.Create(viewModel);
			int finalAtividadeCount = await _context.Atividade.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialAtividadeCount + 1, finalAtividadeCount);
		}


		[Fact]
		public async void Edit_WithValidData_UpdatesAtividade()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var atividade = new Atividade { AtividadeId = Guid.NewGuid(), Name = "Atividade Teste", Description = "Descricao Teste", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2), Picture = "logo.png" };



			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Atividade.Add(atividade);
			_context.SaveChanges();


			var viewModel = new AtividadeEditViewModel
			{
				AtividadeId = atividade.AtividadeId,
				Name = "Atividade Editada",
				Description = "Descricao Editada",
				StartDate = DateTime.Now.AddDays(3),
				EndDate = DateTime.Now.AddDays(5)
			};
			
			// Act
			var result = await _controller.Edit(atividade.AtividadeId, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedAtividade = await _context.Atividade.FindAsync(atividade.AtividadeId);
			Assert.Equal(viewModel.Name, updatedAtividade.Name);
			Assert.Equal(viewModel.Description, updatedAtividade.Description);
			Assert.Equal(viewModel.StartDate, updatedAtividade.StartDate);
			Assert.Equal(viewModel.EndDate, updatedAtividade.EndDate);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesRefeicao()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var atividade = new Atividade { AtividadeId = Guid.NewGuid(), Name = "Atividade Teste", Description = "Descricao Teste", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2), Picture = "logo.png" };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Atividade.Add(atividade);
			_context.SaveChanges();

			// Act
			int initialAtividadeCount = await _context.Atividade.CountAsync();
			var result = await _controller.DeleteConfirmed(atividade.AtividadeId);
			int finalAtividadeCount = await _context.Atividade.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialAtividadeCount - 1, finalAtividadeCount);
		}
	}
}