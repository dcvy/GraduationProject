using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using Kclinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KclinicWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string pythonScriptPath; // Replace with the actual path to your Python script
    private readonly string pythonExecutable; // Replace with the Python executable if not in system PATH
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStorageProvider storageProvider;

    public HomeController(IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IStorageProvider storageProvider)
    {
        _webHostEnvironment = webHostEnvironment;
        pythonScriptPath = "D:/codes/python/image_main" +
            "/classify.py"; // Replace with the actual path to your Python script
        pythonExecutable = "C:/Users/ADMIN/AppData/Local/Programs/Python/Python37/python.exe"; // Replace with the Python executable if not in system PATH
        _logger = logger;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        this.storageProvider = storageProvider;
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
        Task.Run(async () =>
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/UserClaims");
                    response.EnsureSuccessStatusCode();

                    // Process the response if needed
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // You can do something with the response data here
                }
                catch (HttpRequestException ex)
                {
                    // Handle exceptions
                    Console.WriteLine($"Error calling API: {ex.Message}");
                }
            }
        });
        return View(viewModel);
    }

    [HttpGet("/api/UserClaims")]
    [Authorize]
    public IActionResult GetUserClaims()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.Claims;

        // Create a dictionary to store claims
        var claimsDictionary = new Dictionary<string, string>();

        // Populate the dictionary with claim type-value pairs
        foreach (var claim in claims)
        {
            claimsDictionary.Add(claim.Type, claim.Value);
        }

        // Save the claims as JSON into storage
        var jsonClaims = JsonConvert.SerializeObject(claimsDictionary);
        storageProvider.SaveJson("userClaims.json", jsonClaims);

        // Return the claims as JSON
        return Json(claimsDictionary);
    }

    [HttpGet("/api/User")]
    public IActionResult GetUser()
    {
        // Check if claims are already saved, if not, call /api/UserClaims to retrieve and save them
        var savedJson = storageProvider.RetrieveJson("userClaims.json");
        if (savedJson == null)
        {
            // Retrieve the claims from /api/UserClaims and save them
            var userClaimsResponse = GetUserClaims() as JsonResult;
            var userClaims = userClaimsResponse?.Value;

            // Save the claims as JSON into storage
            var jsonClaims = JsonConvert.SerializeObject(userClaims);
            storageProvider.SaveJson("userClaims.json", jsonClaims);

            // Set savedJson to the newly saved JSON
            savedJson = jsonClaims;
        }

        // Check if savedJson is not null before deserializing
        if (savedJson != null)
        {
            // Parse the JSON into a dictionary
            var claimsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(savedJson);

            // Return the claims as JSON
            return Json(claimsDictionary);
        }
        else
        {
            // Handle the case where savedJson is null
            return NotFound("User claims not found");
        }
    }
    public interface IStorageProvider
    {
        void SaveJson(string fileName, string json);
        string RetrieveJson(string fileName);
    }

    // Simple in-memory storage provider (for demonstration purposes)
    public class InMemoryStorageProvider : IStorageProvider
    {
        private readonly Dictionary<string, string> storage = new Dictionary<string, string>();

        public void SaveJson(string fileName, string json)
        {
            storage[fileName] = json;
        }

        public string RetrieveJson(string fileName)
        {
            return storage.TryGetValue(fileName, out var json) ? json : null;
        }
    }

    [HttpGet("/api/CateItems")]
    public IActionResult GetCateItems()
    {
        var cateItems = _unitOfWork.CateItem.GetAll().ToList();

        // Return the CateItems as JSON
        return Json(cateItems);
    }

    [HttpGet("/api/Products")]
    public IActionResult GetProducts([FromQuery(Name = "CateItem")] string cateItem)
    {
        var productsQuery = _unitOfWork.Product.GetAll(includeProperties: "CateItem");

        if (!string.IsNullOrEmpty(cateItem))
        {
            productsQuery = productsQuery.Where(p => p.CateItem.Name.ToLower() == cateItem.ToLower());
        }

        var products = productsQuery.ToList();

        return Json(products);
    }


    private string ExecutePythonScript(string scriptPath, string pythonExecutable, string imagePath)
    {
        try
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
                StringBuilder resultBuilder = new StringBuilder();
                StringBuilder errorBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data) && !e.Data.Contains("|"))  // Exclude lines with progress bar
                    {
                        resultBuilder.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                string result = resultBuilder.ToString().Trim();
                string error = errorBuilder.ToString().Trim();

                if (!string.IsNullOrEmpty(error))
                {
                    // Log the error or handle it as needed
                    _logger.LogError($"Error during Python script execution: {error}");
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            _logger.LogError($"Exception during Python script execution: {ex.Message}");
            return "An error occurred during image processing.";
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
        return RedirectToAction("Index","Cart");
    }


    [HttpPost("/api/Details")]
    public async Task<IActionResult> DetailsApi([FromBody] ShoppingCart shoppingCart)
    {
        if (shoppingCart == null)
        {
            // Handle invalid or missing JSON data
            return BadRequest("Invalid or missing JSON data");
        }

        // Retrieve user claims from /api/UserClaims
        var userClaims = await GetUserClaimsAsync();

        // Check if 'nameidentifier' and 'name' claims are present
        if (userClaims.ContainsKey(ClaimTypes.NameIdentifier) && userClaims.ContainsKey(ClaimTypes.Name))
        {
            shoppingCart.ApplicationUserId = userClaims[ClaimTypes.NameIdentifier];

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == shoppingCart.ApplicationUserId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            }

            _unitOfWork.Save();
            return Ok("Product added successfully");
        }
        else
        {
            // Handle the case where 'nameidentifier' or 'name' claims are missing
            return BadRequest("User ID or email not found in claims");
        }
    }

    // Method to get user claims from /api/UserClaims
    private async Task<Dictionary<string, string>> GetUserClaimsAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var userClaimsEndpoint = "https://localhost:5001/api/User";

            var response = await httpClient.GetAsync(userClaimsEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var claimsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                return claimsDictionary;
            }
            else
            {
                // Handle non-success status codes
                Console.WriteLine($"Error getting user claims. Status code: {response.StatusCode}");
                return new Dictionary<string, string>(); // or handle the error appropriately
            }
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error getting user claims: {ex.Message}");
            return new Dictionary<string, string>(); // or handle the error appropriately
        }
    }
    /*
        [HttpPost("/api/Details2")]
        [Authorize]
        public async Task<IActionResult> DetailsApi2s([FromBody] ShoppingCart shoppingCart)
        {
            if (shoppingCart == null)
            {
                // Handle invalid or missing JSON data
                return BadRequest("Invalid or missing JSON data");
            }

            // Request user claims from /api/UserClaims
            var userClaims = await GetUserClaimsAsync();

            // Check if 'NameIdentifier' claim is present
            if (userClaims.ContainsKey(ClaimTypes.NameIdentifier))
            {
                // Assign the user's ID to the shopping cart
                shoppingCart.ApplicationUserId = userClaims[ClaimTypes.NameIdentifier];
            }
            else
            {
                // Handle the case where 'NameIdentifier' claim is missing
                return BadRequest("User ID not found in claims");
            }

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == shoppingCart.ApplicationUserId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            }

            _unitOfWork.Save();
            return RedirectToAction("Index", "Cart");
        }

        // Method to get user claims from /api/UserClaims
        private async Task<Dictionary<string, string>> GetUserClaimsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Replace 'https://your-api-domain/api/UserClaims' with the actual URL of your /api/UserClaims endpoint
            var userClaimsEndpoint = "https://localhost:5001/api/UserClaims";

            var response = await httpClient.GetAsync(userClaimsEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var claimsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                return claimsDictionary;
            }

            // Handle the case where the request to /api/UserClaims fails
            throw new HttpRequestException($"Error getting user claims. Status code: {response.StatusCode}");
        }
    */
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
