using Kclinic.DataAccess;
using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using Kclinic.Models.ViewModels;
using Kclinic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KclinicWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class BlogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        BlogVM blogVM = new()
        {
            Blog = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };

        if (id == null || id == 0)
        {
            //create blog
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(blogVM);
        }
        else
        {
            blogVM.Blog = _unitOfWork.Blog.GetFirstOrDefault(u => u.Id == id);
            return View(blogVM);

            //update blog
        }


    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(BlogVM obj, IFormFile? file)
    {
		if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\blogs");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Blog.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Blog.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.Blog.ImageUrl = @"\images\blogs\" + fileName + extension;

            }
            if (obj.Blog.Id == 0)
            {
                _unitOfWork.Blog.Add(obj.Blog);
            }
            else
            {
                _unitOfWork.Blog.Update(obj.Blog);
            }
            _unitOfWork.Save();
            TempData["success"] = "Blog created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }



    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var blogList = _unitOfWork.Blog.GetAll(includeProperties: "Category,CoverType");
        return Json(new { data = blogList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Blog.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Blog.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
