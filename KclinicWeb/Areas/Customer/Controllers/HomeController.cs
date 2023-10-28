using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using Kclinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace KclinicWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
		var viewModel = new HomeVM
		{
			Blogs = _unitOfWork.Blog.GetAll(includeProperties: "Category,CoverType"),
			Products = _unitOfWork.Product.GetAll(includeProperties: "CateItem"),
			Launchs = _unitOfWork.Launch.GetAll(),
            Abouts = _unitOfWork.About.GetAll(),
            Functions = _unitOfWork.Function.GetAll(),
            Features = _unitOfWork.Feature.GetAll(),
            Partners = _unitOfWork.Partner.GetAll(),
            ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(),
        };
        var num = 0;
        foreach(var item in viewModel.ShoppingCarts)
        {
            num++;
        }
        Response.Cookies.Append("CartItemCount", num.ToString());
        if (num == 0)
        {
            Response.Cookies.Delete("CartItemCount");
        }
        return View(viewModel);
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
