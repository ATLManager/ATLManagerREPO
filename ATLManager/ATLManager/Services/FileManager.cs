using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Services
{
	public class FileManager : IFileManager
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		public FileManager(IWebHostEnvironment webHostEnvironment) 
		{ 
			_webHostEnvironment = webHostEnvironment;
		}

		public string UploadFile(IFormFile file, string folderName)
		{
			if (file != null)
			{
				string uploadsFolder;
				if (Path.GetExtension(file.FileName) == ".pdf")
				{
					uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, @"files\users\" + folderName);
				}
				else
				{
					uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, @"images\uploads\" + folderName);
				}

				if (uploadsFolder == null)
					return null;

				string uniqueFileName = Guid.NewGuid().ToString() + "_id_" + file.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using var fileStream = new FileStream(filePath, FileMode.Create);
					file.CopyTo(fileStream);

				return uniqueFileName;
			}
			return null;
		}
	}
}
