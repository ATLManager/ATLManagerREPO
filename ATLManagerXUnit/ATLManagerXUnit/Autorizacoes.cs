using ATLManager.Controllers;
using ATLManager.Data;
using ATLManager.Models;
using ATLManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATLManagerXUnit
{
	public class EducandoResponsaveisControllerTest
	{

		private readonly ATLManagerAuthContext _context;
		private readonly EducandoResponsaveisController _controller;
		private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;

		public EducandoResponsaveisControllerTest()
		{
			var options = new DbContextOptionsBuilder<ATLManagerAuthContext>()
				.UseInMemoryDatabase(databaseName: "ATLManagerAuthContext")
				.Options;
			_context = new ATLManagerAuthContext(options);

			_webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

			_controller = new EducandoResponsaveisController(_context, _webHostEnvironmentMock.Object);
		}

		[Fact]
		public async Task Create_WithValidData_CreatesResponsavel()
		{
			// Arrange
			var educando = new Educando
			{
				EducandoId = Guid.NewGuid(),
				Name = "Jose",
				Apelido = "Silva",
				BirthDate = DateTime.Parse("2000-01-01"),
				CC = "123456789",
				ProfilePicture = "logo.png",
				AtlId = Guid.NewGuid(),
				EncarregadoId = Guid.NewGuid(),
				Genero = "Masculino"
			};

			_context.Educando.Add(educando);
			_context.SaveChanges();

			var viewModel = new ResponsavelCreateViewModel(educando.EducandoId)
			{
				Name = "Name",
				Apelido = "Apelido",
				CC = "123456789",
				Phone = "923456789",
				Parentesco = "Parentesco",
				ProfilePicture = null
			};

			// Act
			var result = await _controller.Create(viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("DetailsResponsaveis", redirectToActionResult.ActionName);
			Assert.Equal("Educandos", redirectToActionResult.ControllerName);
			Assert.Equal(educando.EducandoId, redirectToActionResult.RouteValues["id"]);

			var responsavel = await _context.EducandoResponsavel.FirstOrDefaultAsync(r => r.EducandoId == educando.EducandoId);
			Assert.NotNull(responsavel);
			Assert.Equal(viewModel.Name, responsavel.Name);
			Assert.Equal(viewModel.Apelido, responsavel.Apelido);
			Assert.Equal(viewModel.CC, responsavel.CC);
			Assert.Equal(viewModel.Phone, responsavel.Phone);
			Assert.Equal(viewModel.Parentesco, responsavel.Parentesco);
		}

		[Fact]
		public async Task Edit_WithValidData_UpdatesResponsavel()
		{
			// Arrange
			var educando = new Educando
			{
				EducandoId = Guid.NewGuid(),
				Name = "Jose",
				Apelido = "Silva",
				BirthDate = DateTime.Parse("2000-01-01"),
				CC = "123456789",
				ProfilePicture = "logo.png",
				AtlId = Guid.NewGuid(),
				EncarregadoId = Guid.NewGuid(),
				Genero = "Masculino"
			};

			_context.Educando.Add(educando);

			var responsavel = new EducandoResponsavel
			{
				EducandoResponsavelId = Guid.NewGuid(),
				EducandoId = educando.EducandoId,
				Name = "Old Name",
				Apelido = "Old Apelido",
				CC = "987654321",
				Phone = "912345678",
				Parentesco = "Old Parentesco",
				ProfilePicture = "old_logo.png"
			};

			_context.EducandoResponsavel.Add(responsavel);
			_context.SaveChanges();

			var viewModel = new ResponsavelEditViewModel
			{
				EducandoResponsavelId = responsavel.EducandoResponsavelId,
				EducandoId = responsavel.EducandoId,
				Name = "New Name",
				Apelido = "New Apelido",
				CC = "123456789",
				Phone = "923456789",
				Parentesco = "New Parentesco",
				ProfilePicture = null
			};

			// Act
			var result = await _controller.Edit(responsavel.EducandoResponsavelId, viewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("DetailsResponsaveis", redirectToActionResult.ActionName);
			Assert.Equal("Educandos", redirectToActionResult.ControllerName);
			Assert.Equal(educando.EducandoId, redirectToActionResult.RouteValues["id"]);

			var updatedResponsavel = await _context.EducandoResponsavel.FindAsync(responsavel.EducandoResponsavelId);
			Assert.NotNull(updatedResponsavel);
			Assert.Equal(viewModel.Name, updatedResponsavel.Name);
			Assert.Equal(viewModel.Apelido, updatedResponsavel.Apelido);
			Assert.Equal(viewModel.CC, updatedResponsavel.CC);
			Assert.Equal(viewModel.Phone, updatedResponsavel.Phone);
			Assert.Equal(viewModel.Parentesco, updatedResponsavel.Parentesco);
		}

		[Fact]
		public async Task DeleteConfirmed_WithValidId_DeletesResponsavel()
		{
			// Arrange
			var educando = new Educando
			{
				EducandoId = Guid.NewGuid(),
				Name = "Jose",
				Apelido = "Silva",
				BirthDate = DateTime.Parse("2000-01-01"),
				CC = "123456789",
				ProfilePicture = "logo.png",
				AtlId = Guid.NewGuid(),
				EncarregadoId = Guid.NewGuid(),
				Genero = "Masculino"
			};

			_context.Educando.Add(educando);

			var responsavel = new EducandoResponsavel
			{
				EducandoResponsavelId = Guid.NewGuid(),
				EducandoId = educando.EducandoId,
				Name = "Name",
				Apelido = "Apelido",
				CC = "987654321",
				Phone = "912345678",
				Parentesco = "Parentesco",
				ProfilePicture = "old_logo.png"
			};

			_context.EducandoResponsavel.Add(responsavel);
			_context.SaveChanges();

			// Act
			var result = await _controller.DeleteConfirmed(responsavel.EducandoResponsavelId);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("DetailsResponsaveis", redirectToActionResult.ActionName);
			Assert.Equal("Educandos", redirectToActionResult.ControllerName);
			Assert.Equal(educando.EducandoId, redirectToActionResult.RouteValues["id"]);

			var deletedResponsavel = await _context.EducandoResponsavel.FindAsync(responsavel.EducandoResponsavelId);
			Assert.Null(deletedResponsavel);
		}

	}
}
