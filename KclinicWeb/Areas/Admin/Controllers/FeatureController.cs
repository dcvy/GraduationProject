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
public class FeatureController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public FeatureController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        FeatureVM FeatureVM = new()
        {
            Feature = new(),
        };

        if (id == null || id == 0)
        {
            return View(FeatureVM);
        }
        else
        {
            FeatureVM.Feature = _unitOfWork.Feature.GetFirstOrDefault(u => u.Id == id);
            return View(FeatureVM);
        }


    }

	//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(FeatureVM obj, IFormFile? file)
	{
		if (ModelState.IsValid)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(wwwRootPath, @"images\features");
				var extension = Path.GetExtension(file.FileName);

				if (obj.Feature.ImageUrl != null)
				{
					var oldImagePath = Path.Combine(wwwRootPath, obj.Feature.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStreams);
				}
				obj.Feature.ImageUrl = @"\images\features\" + fileName + extension;

			}
			if (obj.Feature.Id == 0)
			{
				_unitOfWork.Feature.Add(obj.Feature);
			}
			else
			{
				_unitOfWork.Feature.Update(obj.Feature);
			}
			_unitOfWork.Save();
			TempData["success"] = "Feature created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}



	#region API CALLS
	[HttpGet]
    public IActionResult GetAll()
    {
        var FeatureList = _unitOfWork.Feature.GetAll();
        return Json(new { data = FeatureList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Feature.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Feature.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
