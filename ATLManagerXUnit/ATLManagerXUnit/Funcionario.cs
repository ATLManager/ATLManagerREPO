//using System;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using ATLManager.Areas.Identity.Data;
//using ATLManager.Controllers;
//using ATLManager.Data;
//using ATLManager.Models;
//using ATLManager.ViewModels;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging.Abstractions;
//using Moq;
//using Xunit;

//namespace ATLManager.Tests
//{
//	public class FuncionariosControllerTests
//	{
//		private readonly ATLManagerAuthContext _context;
//		private readonly FuncionariosController _controller;
//		private readonly Mock<UserManager<ATLManagerUser>> _userManagerMock;
//		private readonly Mock<IUserStore<ATLManagerUser>> _userStoreMock;
//		private readonly Mock<IUserEmailStore<ATLManagerUser>> _emailStoreMock;
//		private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;

//		public FuncionariosControllerTests()
//		{
//			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>()
//				.UseInMemoryDatabase(databaseName: "ATLManagerAuthContext")
//				.Options;

//			_context = new ATLManagerAuthContext(options);

//			_userStoreMock = new Mock<IUserStore<ATLManagerUser>>();

//			_emailStoreMock = new Mock<IUserEmailStore<ATLManagerUser>>();
//			_emailStoreMock.Setup(m => m.SetEmailAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(IdentityResult.Success));

//			_userManagerMock = new Mock<UserManager<ATLManagerUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
//			_userManagerMock.Setup(x => x.SupportsUserEmail).Returns(true);
//			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ATLManagerUser());
//			_userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ATLManagerUser>())).ReturnsAsync("token");
//			_userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

//			_webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

//			_controller = new FuncionariosController(_context, _userManagerMock.Object, _userStoreMock.Object, _webHostEnvironmentMock.Object);

//		}

//		[Fact]
//		public async Task Create_ValidData_CreatesFuncionario()
//		{
//			// Arrange
//			var userId = "1";
//			var user = new ATLManagerUser { Id = userId, UserName = "user1", FirstName = "Test", LastName = "User", Email = "test@example.com", Picture = "logo.png" };
//			var userAccount = new ContaAdministrativa { UserId = user.Id, ContaId = Guid.NewGuid(), DateOfBirth = DateTime.Parse("2000-01-01"), CC = "123456789", ProfilePicture = "logo.png", AtlId = Guid.NewGuid() };
//			var atl = new ATL { AtlId = Guid.NewGuid(), Name = "ATL Teste", Address = "Morada Teste", City = "Cidade Teste", PostalCode = "1234-567", NIPC = "500000000", LogoPicture = "example_logo.png" };

//			_userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
//			_userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
//			_userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
//			_userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ATLManagerUser>())).ReturnsAsync("token");
//			_userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

//			_userStoreMock.As<IUserEmailStore<ATLManagerUser>>().Setup(m => m.SetEmailAsync(It.IsAny<ATLManagerUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(IdentityResult.Success));

//			_context.Users.Add(user);
//			_context.ATL.Add(atl);
//			_context.SaveChanges();

//			var viewModel = new FuncionarioCreateViewModel
//			{
//				FirstName = "John",
//				LastName = "Doe",
//				Email = "john.doe@example.com",
//				Password = "P@ssw0rd!",
//				CC = "123456789",
//				DateOfBirth = new DateTime(1990, 1, 1),
//				ProfilePicture = new FormFile(new MemoryStream(), 0, 0, "ProfilePicture", "profilePicture.png")
//			};

//			// Define the fake HttpContext
//			var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
//			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//			var httpContext = new DefaultHttpContext { User = claimsPrincipal };
//			_controller.ControllerContext.HttpContext = httpContext;

//			// Act
//			var result = await _controller.Create(viewModel);

//			// Assert
//			Assert.IsType<RedirectToActionResult>(result);
//			var createdFuncionario = _context.ContaAdministrativa.FirstOrDefault(f => f.CC == viewModel.CC);
//			Assert.NotNull(createdFuncionario);
//			Assert.Equal(viewModel.FirstName, createdFuncionario.User.FirstName);
//			Assert.Equal(viewModel.LastName, createdFuncionario.User.LastName);
//			Assert.Equal(viewModel.DateOfBirth, createdFuncionario.DateOfBirth);
//		}
//	}
//}