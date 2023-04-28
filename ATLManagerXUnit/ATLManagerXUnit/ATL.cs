using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ATLManager.Tests
{
	public class ATLControllerTests
	{
		private readonly ATLController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public ATLControllerTests()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>().UseInMemoryDatabase(databaseName: "ATLManagerAuthContext").Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			_controller = new ATLController(_context, _userManagerMock.Object, null);
		}

		[Fact]
		public async Task Create_ValidModel_Success()
		{

			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			
			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.Users.Add(user); // Adicione esta linha
			_context.ContaAdministrativa.Add(userAccount);
			_context.SaveChanges();


			var viewModel = new ATLCreateViewModel
			{
				Name = "ATL Test",
				Address = "Test Address",
				City = "Test City",
				PostalCode = "1234-567",
				NIPC = "500000000",
				AgrupamentoId = null,
			};
			
			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			int initialATLCount = await _context.ATL.CountAsync();
			var result = await _controller.Create(viewModel);
			int finalATLCount = await _context.ATL.CountAsync();
			
			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialATLCount + 1, finalATLCount);

		}

		[Fact]
		public async void Edit_WithValidData_UpdatesATL()
		{

			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };
			var atl = new ATL { AtlId = Guid.NewGuid(), Name = "ATL Teste", Address = "Morada Teste", City = "Cidade Teste", PostalCode = "1234-567", NIPC = "500000000", LogoPicture = "example_logo.png" }; // Adicionado valor para NIPC

			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.ATL.Add(atl);
			_context.SaveChanges();

			var viewModel = new ATLEditViewModel
			{
				AtlId = atl.AtlId,
				Name = "ATL Editado",
				Address = "Morada Editada",
				City = "Cidade Editada",
				PostalCode = "1234-567",
				NIPC = "500000000"
			};

			// Act
			var result = await _controller.Edit(atl.AtlId, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			var updatedATL = await _context.ATL.FindAsync(atl.AtlId);
			Assert.Equal(viewModel.Name, updatedATL.Name);
			Assert.Equal(viewModel.Address, updatedATL.Address);
			Assert.Equal(viewModel.City, updatedATL.City);
			Assert.Equal(viewModel.PostalCode, updatedATL.PostalCode);
			Assert.Equal(viewModel.NIPC, updatedATL.NIPC);
		}


		[Fact]
		public async void Delete_WithValidId_DeletesATL()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png" };			
			var atl = new ATL { AtlId = Guid.NewGuid(), Name = "ATL Teste", Address = "Morada Teste", City = "Cidade Teste", PostalCode = "1234-567", NIPC = "500000000", LogoPicture = "example_logo.png", }; // Adicionado valor para NIPC e LogoPicture
			
			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.ContaAdministrativa.Add(userAccount);
			_context.ATL.Add(atl);
			_context.SaveChanges();

			// Act
			int initialATLCount = await _context.ATL.CountAsync();
			var result = await _controller.DeleteConfirmed(atl.AtlId);
			int finalATLCount = await _context.ATL.CountAsync();

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(initialATLCount - 1, finalATLCount);
		}

	}
}