using ATLManager.Areas.Identity.Data;
using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
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
	public class Estatisticas
	{
		private readonly EstatisticasController _controller;
		private readonly ATLManagerAuthContext _context;
		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;

		public Estatisticas()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>()
				.UseInMemoryDatabase(databaseName: "ATLManagerAuthContext")
				.Options;
			_context = new ATLManagerAuthContext(options);

			var userStoreMock = new Mock<IUserStore<ATLManagerUser>>();
			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

			
			_controller = new EstatisticasController(_context, _userManagerMock.Object);
		}

		[Fact]
		public async Task Index_ReturnsViewResult()
		{
			// Arrange
			var userId = "1";
			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png", AtlId = Guid.NewGuid() };
			var atl = new ATL { AtlId = Guid.NewGuid(), Name = "ATL Teste", Address = "Morada Teste", City = "Cidade Teste", PostalCode = "1234-567", NIPC = "500000000", LogoPicture = "example_logo.png" }; // Adicionado valor para NIPC
			
			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
			_context.Users.Add(user);
			_context.ATL.Add(atl);
			_context.ContaAdministrativa.Add(userAccount);
			_context.SaveChanges();

			// Define o HttpContext falso
			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
			_controller.ControllerContext.HttpContext = httpContext;

			// Act
			var result = await _controller.Index(atl.AtlId);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsType<EstatisticasViewModel>(viewResult.Model);
		}


	}
}
