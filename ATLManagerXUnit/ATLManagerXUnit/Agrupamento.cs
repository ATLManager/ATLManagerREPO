using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Security.Claims;
using Xunit;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Tests
{
	public class AgrupamentosControllerTests
	{
		private readonly AgrupamentosController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public AgrupamentosControllerTests()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_controller = new AgrupamentosController(_context, _userManagerMock.Object, null);
		}

		[Fact]
		public async void Create_WithValidData_CreatesNewAgrupamento()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.SaveChanges();

			var viewModel = new AgrupamentoCreateViewModel
			{
				Name = "Agrupamento Teste",
				Location = "Local Teste",
				NIPC = "500000000"
			};

			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			int initialAgrupamentoCount = await _context.Agrupamento.CountAsync();
			var result = await _controller.Create(viewModel);
			_context.SaveChanges(); // Adicionado SaveChanges aqui
			int finalAgrupamentoCount = await _context.Agrupamento.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialAgrupamentoCount + 1, finalAgrupamentoCount);
		}


		[Fact]
		public async void Edit_WithValidData_UpdatesAgrupamento()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var agrupamento = new Agrupamento { AgrupamentoID = Guid.NewGuid(), Name = "Agrupamento Teste", Location = "Local Teste", NIPC = "500000000", ContaId = userAccount.ContaId, LogoPicture = "example_logo.png" }; // Adicionado valor para LogoPicture

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Agrupamento.Add(agrupamento);
			_context.SaveChanges();

			var viewModel = new AgrupamentoEditViewModel
			{
				AgrupamentoId = agrupamento.AgrupamentoID,
				Name = "Agrupamento Editado",
				Location = "Local Editado",
				NIPC = "600000000"
			};

			// Act
			var result = await _controller.Edit(agrupamento.AgrupamentoID, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedAgrupamento = await _context.Agrupamento.FindAsync(agrupamento.AgrupamentoID);
			Assert.Equal(viewModel.Name, updatedAgrupamento.Name);
			Assert.Equal(viewModel.Location, updatedAgrupamento.Location);
			Assert.Equal(viewModel.NIPC, updatedAgrupamento.NIPC);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesAgrupamento()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var agrupamento = new Agrupamento { AgrupamentoID = Guid.NewGuid(), Name = "Agrupamento Teste", Location = "Local Teste", NIPC = "500000000", ContaId = userAccount.ContaId, LogoPicture = "example_logo.png" }; // Adicionado valor para LogoPicture

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Agrupamento.Add(agrupamento);
			_context.SaveChanges();

			// Act
			int initialAgrupamentoCount = await _context.Agrupamento.CountAsync();
			var result = await _controller.DeleteConfirmed(agrupamento.AgrupamentoID);
			int finalAgrupamentoCount = await _context.Agrupamento.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialAgrupamentoCount - 1, finalAgrupamentoCount);
		}


	}
}
