using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using Kclinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace KclinicWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string pythonScriptPath; // Replace with the actual path to your Python script
    private readonly string pythonExecutable; // Replace with the Python executable if not in system PATH

    public HomeController(IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _webHostEnvironment = webHostEnvironment;
        pythonScriptPath = "D:/codes/python/image_recognizer" +
            "/classify.py"; // Replace with the actual path to your Python script
        pythonExecutable = "C:/Users/ADMIN/AppData/Local/Programs/Python/Python36/python.exe"; // Replace with the Python executable if not in system PATH
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(string search, int? cateItemId, IFormFile imageFile)
    {
        var viewModel = new HomeVM
        {
            Blogs = _unitOfWork.Blog.GetAll(includeProperties: "Category,CoverType"),
            Launchs = _unitOfWork.Launch.GetAll(),
            Abouts = _unitOfWork.About.GetAll(),
            Functions = _unitOfWork.Function.GetAll(),
            Features = _unitOfWork.Feature.GetAll(),
            Partners = _unitOfWork.Partner.GetAll(),
            CateItems = _unitOfWork.CateItem.GetAll(),
        };

        // Initialize result as an empty string
        string result = string.Empty;

        // Process the image if it's provided in the POST request
        if (Request.Method == HttpMethods.Post)
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
                    result = ExecutePythonScript(pythonScriptPath, pythonExecutable, uploadPath);

                    // Pass the result to the view
                    ViewData["Result"] = result;
                }
                else
                {
                    // Handle the case where the uploaded file is not an image
                    ModelState.AddModelError("imageFile", "The selected file is not an image.");
                }
            }
            else
            {
                // Handle the case where no file was uploaded
                ModelState.AddModelError("imageFile", "Please select an image file.");
            }
        }


        var products = _unitOfWork.Product.GetAll(includeProperties: "CateItem");

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLowerInvariant();
            products = products.Where(p => p.Name.ToLowerInvariant().Contains(search));
        }

        if (cateItemId.HasValue)
        {
            products = products.Where(p => p.CateItemId == cateItemId.Value);
        }

        string searchTerm = !string.IsNullOrWhiteSpace(result) ? result.Split(' ')[0] : string.Empty;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            products = products.Where(p => p.CateItem.Name == searchTerm);
        }

        viewModel.Products = products.ToList();
        viewModel.CateItems = _unitOfWork.CateItem.GetAll().ToList();

        return View(viewModel);
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


    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            ProductId = productId,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "CateItem"),
        };

        return View(cartObj);
    }

    public IActionResult DetailsBlog(int id)
    {
        BlogCount blogObj = new()
        {
            Count = 1,
            Blog = _unitOfWork.Blog.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType"),
        };

      

        var viewModel = new BlogCountVM
        {
            BlogCount = blogObj,
            CoverTypes = _unitOfWork.CoverType.GetAll(),
    };

        return View(viewModel);
    }
    public IActionResult DetailLaunch(int id)
    {
        Launch launchObj = _unitOfWork.Launch.GetFirstOrDefault(u => u.Id == id);
        return View(launchObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);


        if (cartFromDb == null)
        {

            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        else
        {
            _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
        }

        _unitOfWork.Save();

        IEnumerable<ShoppingCart> listCart = _unitOfWork.ShoppingCart.GetAll();
        var num = 0;
        foreach (var item in listCart)
        {
            num++;
        }
        Response.Cookies.Append("CartItemCount", num.ToString());
        if (num == 0)
        {
            Response.Cookies.Delete("CartItemCount");
        }
        return RedirectToAction("Index","Cart");
    }

    public IActionResult Function(int id)
    {
        var viewModel = new FunctionVM
        {
            Function = _unitOfWork.Function.GetFirstOrDefault(u => u.Id == id),
            Functions = _unitOfWork.Function.GetAll()
        };

        return View(viewModel);
    }

    public IActionResult Gallery()
    {
        return View();
    }

    public IActionResult Trial()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
