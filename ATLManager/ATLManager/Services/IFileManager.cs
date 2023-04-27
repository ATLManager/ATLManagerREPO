using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ATLManager.Services
{
	public interface IFileManager
	{
		string UploadFile(IFormFile file, string folderName);
	}
}
