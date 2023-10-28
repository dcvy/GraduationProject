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
public class AboutController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public AboutController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        AboutVM aboutVM = new()
        {
            About = new(),
        };

        if (id == null || id == 0)
        {
            //create about
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(aboutVM);
        }
        else
        {
            aboutVM.About = _unitOfWork.About.GetFirstOrDefault(u => u.Id == id);
            return View(aboutVM);

            //update about
        }


    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(AboutVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\abouts");
                var extension = Path.GetExtension(file.FileName);

                if (obj.About.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.About.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.About.ImageUrl = @"\images\abouts\" + fileName + extension;

            }
            if (obj.About.Id == 0)
            {
                _unitOfWork.About.Add(obj.About);
            }
            else
            {
                _unitOfWork.About.Update(obj.About);
            }
            _unitOfWork.Save();
            TempData["success"] = "About created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }



    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var aboutList = _unitOfWork.About.GetAll();
        return Json(new { data = aboutList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.About.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.About.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}