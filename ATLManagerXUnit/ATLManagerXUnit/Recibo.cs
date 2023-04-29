using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
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
	public class ReciboControllerTest
	{
		private readonly Mock<IEmailSender> _mockEmailSender;
		private readonly Mock<INotificacoesController> _mockNotificacoesController;

		private readonly RecibosController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public ReciboControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_mockEmailSender = new Mock<IEmailSender>();
			_mockNotificacoesController = new Mock<INotificacoesController>();

			_controller = new RecibosController(
				_context,
				_userManagerMock.Object,
				_mockEmailSender.Object,
				_mockNotificacoesController.Object
			);
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


			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_userManagerMock.Setup(x => x.GetEmailAsync(It.IsAny<ATLManagerUser>())).ReturnsAsync("test@example.com");

			_mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
			_mockNotificacoesController.Setup(x => x.CreateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

			var viewModel = new ReciboCreateViewModel
			{
				Name = "Test Recibo",
				Price = "100.0",
				NIB = "123456789",
				Description = "Test Description",
				DateLimit = DateTime.UtcNow.AddDays(1),
				Educando = null
			};


			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;
			
			// Act
			var result = await _controller.Create(viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(RecibosController.Index), redirectToActionResult.ActionName);
		}

		[Fact]
		public async void Edit_WithValidData_UpdatesRecibo()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var recibo = new Recibo { ReciboId = Guid.NewGuid(), Name = "Recibo Teste", Description = "Descricao Teste", DateLimit = DateTime.Now.AddDays(10), NIB = "123456789012345678901", Price = "100.0" };


			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Recibo.Add(recibo);
			_context.SaveChanges();


			var viewModel = new ReciboEditViewModel
			{
				ReciboId = recibo.ReciboId,
				Name = "Recibo Editado",
				Description = "Descricao Editada",
				Price = "150.0",
				NIB = "123456789012345678901",
				DateLimit = DateTime.Now.AddDays(15)
			};

			// Act
			var result = await _controller.Edit(recibo.ReciboId, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedRecibo = await _context.Recibo.FindAsync(recibo.ReciboId);
			Assert.Equal(viewModel.Name, updatedRecibo.Name);
			Assert.Equal(viewModel.Description, updatedRecibo.Description);
			Assert.Equal(viewModel.Price, updatedRecibo.Price);
			Assert.Equal(viewModel.NIB, updatedRecibo.NIB);
			Assert.Equal(viewModel.DateLimit, updatedRecibo.DateLimit);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesRecibo()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var recibo = new Recibo { ReciboId = Guid.NewGuid(), Name = "Recibo Teste", Description = "Descricao Teste", DateLimit = DateTime.Now.AddDays(10), NIB = "123456789012345678901", Price = "100.0" };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.Recibo.Add(recibo);
			_context.SaveChanges();

			// Act
			int initialReciboCount = await _context.Recibo.CountAsync();
			var result = await _controller.DeleteConfirmed(recibo.ReciboId);
			int finalReciboCount = await _context.Recibo.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialReciboCount - 1, finalReciboCount);
		}
	}
}
