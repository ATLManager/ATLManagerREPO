using System;
using System.Security.Claims;
using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SendGrid.Helpers.Mail;
using Xunit;

namespace ATLManager.Tests
{
	public class RefeicaoControllerTest
	{
		private readonly RefeicoesController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public RefeicaoControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_controller = new RefeicoesController(_context, _userManagerMock.Object, null);

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

			var viewModel = new RefeicaoCreateViewModel
			{
				Name = "Refeicao Test",
				Categoria = "Categoria Test",
				Data = DateTime.Now.AddDays(1),
				Descricao = "Descricao Test",
				Proteina = "10",
				HidratosCarbono = "20",
				VR = "30",
				Acucar = "40",
				Lipidos = "50",
				ValorEnergetico = "60",
				AGSat = "70",
				Sal = "80",
			};

			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			int initialRefeicaoCount = await _context.Refeicao.CountAsync();
			var result = await _controller.Create(viewModel);
			int finalRefeicaoCount = await _context.Refeicao.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialRefeicaoCount + 1, finalRefeicaoCount);
		}


		[Fact]
		public async void Edit_WithValidData_UpdatesRefeicao()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var refeicao = new Refeicao { RefeicaoId = Guid.NewGuid(), Name = "Refeicao Teste", Categoria = "Categoria Teste", Data = DateTime.Now.AddDays(1), Descricao = "Descricao Teste", Proteina = "10", HidratosCarbono = "20", VR = "30", Acucar = "40", Lipidos = "50", ValorEnergetico = "60", AGSat = "70", Sal = "80" , Picture = "logo.png"};

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Refeicao.Add(refeicao);
			_context.SaveChanges();

			var viewModel = new RefeicaoEditViewModel
			{
				RefeicaoId = refeicao.RefeicaoId,
				Name = "Refeicao Editada",
				Categoria = "Categoria Editada",
				Data = "04/05/2023",
				Descricao = "Descricao Editada",
				Proteina = "10",
				HidratosCarbono = "20",
				VR = "30",
				Acucar = "40",
				Lipidos = "50",
				ValorEnergetico = "60",
				AGSat = "70",
				Sal = "80",
			};

			// Act
			var result = await _controller.Edit(refeicao.RefeicaoId, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedRefeicao = await _context.Refeicao.FindAsync(refeicao.RefeicaoId);
			Assert.Equal(viewModel.Name, updatedRefeicao.Name);
			Assert.Equal(viewModel.Categoria, updatedRefeicao.Categoria);
			Assert.Equal(viewModel.Data, updatedRefeicao.Data.ToString("dd/MM/yyyy"));
			Assert.Equal(viewModel.Descricao, updatedRefeicao.Descricao);
			Assert.Equal(viewModel.Proteina, updatedRefeicao.Proteina);
			Assert.Equal(viewModel.HidratosCarbono, updatedRefeicao.HidratosCarbono);
			Assert.Equal(viewModel.VR, updatedRefeicao.VR);
			Assert.Equal(viewModel.Acucar, updatedRefeicao.Acucar);
			Assert.Equal(viewModel.Lipidos, updatedRefeicao.Lipidos);
			Assert.Equal(viewModel.ValorEnergetico, updatedRefeicao.ValorEnergetico);
			Assert.Equal(viewModel.AGSat, updatedRefeicao.AGSat);
			Assert.Equal(viewModel.Sal, updatedRefeicao.Sal);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesRefeicao()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var refeicao = new Refeicao { RefeicaoId = Guid.NewGuid(), Name = "Refeicao Teste", Categoria = "Categoria Teste", Data = DateTime.Now.AddDays(1), Descricao = "Descricao Teste", Proteina = "10", HidratosCarbono = "20", VR = "30", Acucar = "40", Lipidos = "50", ValorEnergetico = "60", AGSat = "70", Sal = "80", Picture = "logo.png" };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Refeicao.Add(refeicao);
			_context.SaveChanges();

			// Act
			int initialRefeicaoCount = await _context.Refeicao.CountAsync();
			var result = await _controller.DeleteConfirmed(refeicao.RefeicaoId);
			int finalRefeicaoCount = await _context.Refeicao.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialRefeicaoCount - 1, finalRefeicaoCount);
		}
	}
}