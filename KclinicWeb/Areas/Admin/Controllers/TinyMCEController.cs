// In your ASP.NET Core controller or API controller
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KclinicWeb.Controllers;
[Area("Admin")]
public class TinyMCEController : Controller
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public TinyMCEController(IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
	}

	[HttpPost("UploadImage")]
	public async Task<IActionResult> UploadImage(IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file selected");

		try
		{
			var fileName = $"{Guid.NewGuid()}_{file.FileName}";
			var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "blogs", fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var imageUrl = Url.Content($"~/images/blogs/{fileName}");
			return Ok(new { location = imageUrl });
		}
		catch (Exception ex)
		{
			return BadRequest($"Error uploading the image: {ex.Message}");
		}
	}
    [HttpPost("UploadImageLaunch")]
    public async Task<IActionResult> UploadImageLaunch(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file selected");

        try
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "launchs", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = Url.Content($"~/images/launchs/{fileName}");
            return Ok(new { location = imageUrl });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error uploading the image: {ex.Message}");
        }
    }
}