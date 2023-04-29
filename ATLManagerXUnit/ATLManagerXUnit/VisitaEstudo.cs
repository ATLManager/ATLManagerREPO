using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
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
	public class VisitaEstudoControllerTest
	{
		private readonly VisitasEstudoController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public VisitaEstudoControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_controller = new VisitasEstudoController(_context, _userManagerMock.Object, null);
			_context.Database.EnsureDeleted();
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

			var viewModel = new VisitaEstudoCreateViewModel
			{
				Name = "Visita",
				Date = DateTime.Now.AddDays(1),
				Descripton = "Teste",
				Location = "Setúbal"
			};
			
		

			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			int initialVisitaEstudoCount = await _context.VisitaEstudo.CountAsync();
			var result = await _controller.Create(viewModel);
			int finalVisitaEstudoCount = await _context.VisitaEstudo.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialVisitaEstudoCount + 1, finalVisitaEstudoCount);
		}


		[Fact]
		public async void Edit_WithValidData_UpdatesVisitaEstudo()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var visitaEstudo = new VisitaEstudo { VisitaEstudoID = Guid.NewGuid(), Name = "VisitaTeste", Description = "Teste", Date = DateTime.Now.AddDays(1), Location = "Setúbal", Picture = "logo.png" };


			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.VisitaEstudo.Add(visitaEstudo);
			_context.SaveChanges();


			var viewModel = new VisitaEstudoEditViewModel
			{
				VisitaEstudoID = visitaEstudo.VisitaEstudoID,
				Name = "Visita Editada",
				Descripton = "Descricao Editada",
				Date = "04/05/2023",
				Location = "Setúbal",
			};


			// Act
			var result = await _controller.Edit(visitaEstudo.VisitaEstudoID, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedVisitaEstudo = await _context.VisitaEstudo.FindAsync(visitaEstudo.VisitaEstudoID);
			Assert.Equal(viewModel.Name, updatedVisitaEstudo.Name);
			Assert.Equal(viewModel.Descripton, updatedVisitaEstudo.Description);
			Assert.Equal(viewModel.Date, updatedVisitaEstudo.Date.ToString("dd/MM/yyyy"));
			Assert.Equal(viewModel.Location, updatedVisitaEstudo.Location);
		}

		[Fact]
		public async void Delete_WithValidId_DeletesVisitaEstudo()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var visitaEstudo = new VisitaEstudo { VisitaEstudoID = Guid.NewGuid(), Name = "VisitaTeste", Description = "Teste", Date = DateTime.Now.AddDays(1), Location = "Setúbal", Picture = "logo.png" };

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.VisitaEstudo.Add(visitaEstudo);
			_context.SaveChanges();

			// Act
			int initialVisitaEstudoCount = await _context.VisitaEstudo.CountAsync();
			var result = await _controller.DeleteConfirmed(visitaEstudo.VisitaEstudoID);
			int finalVisitaEstudoCount = await _context.VisitaEstudo.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialVisitaEstudoCount - 1, finalVisitaEstudoCount);
		}
	}
}
