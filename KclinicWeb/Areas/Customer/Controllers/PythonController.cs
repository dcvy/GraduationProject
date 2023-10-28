using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System;

namespace KclinicWeb.Areas.Customer.Controllers;
[Area("Customer")]


public class PythonController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string pythonScriptPath; // Replace with the actual path to your Python script
    private readonly string pythonExecutable; // Replace with the Python executable if not in system PATH

    public PythonController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        pythonScriptPath = "D:/codes/python/sign-language-alphabet-recognizer-master/classify.py"; // Replace with the actual path to your Python script
        pythonExecutable = "C:/Users/ADMIN/AppData/Local/Programs/Python/Python36/python.exe"; // Replace with the Python executable if not in system PATH
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ProcessImage(IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            if (imageFile.ContentType.Contains("image"))
            {
                // Generate a unique file name for the uploaded image
                var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", uniqueFileName);

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Call your Python script to process the image
                string result = ExecutePythonScript(pythonScriptPath, pythonExecutable, uploadPath);

                // Pass the result to the view
                ViewData["Result"] = result;

                return View("Index");
            }
            else
            {
                // Handle the case where the uploaded file is not an image
                ModelState.AddModelError("imageFile", "The selected file is not an image.");
                return View("Index");
            }
        }
        else
        {
            // Handle the case where no file was uploaded
            ModelState.AddModelError("imageFile", "Please select an image file.");
            return View("Index");
        }
    }

    private string ExecutePythonScript(string scriptPath, string pythonExecutable, string imagePath)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = pythonExecutable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"{scriptPath} \"{imagePath}\""
        };

        using (var process = new Process { StartInfo = processInfo })
        {
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
