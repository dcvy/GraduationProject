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
public class LaunchController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public LaunchController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        LaunchVM LaunchVM = new()
        {
            Launch = new(),
        };

        if (id == null || id == 0)
        {
            return View(LaunchVM);
        }
        else
        {
            LaunchVM.Launch = _unitOfWork.Launch.GetFirstOrDefault(u => u.Id == id);
            return View(LaunchVM);
        }


    }

	//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(LaunchVM obj, IFormFile? file)
	{
		if (ModelState.IsValid)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(wwwRootPath, @"images\launchs");
				var extension = Path.GetExtension(file.FileName);

				if (obj.Launch.ImageUrl != null)
				{
					var oldImagePath = Path.Combine(wwwRootPath, obj.Launch.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStreams);
				}
				obj.Launch.ImageUrl = @"\images\launchs\" + fileName + extension;

			}
			if (obj.Launch.Id == 0)
			{
				_unitOfWork.Launch.Add(obj.Launch);
			}
			else
			{
				_unitOfWork.Launch.Update(obj.Launch);
			}
			_unitOfWork.Save();
			TempData["success"] = "Launch created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}



	#region API CALLS
	[HttpGet]
    public IActionResult GetAll()
    {
        var LaunchList = _unitOfWork.Launch.GetAll();
        return Json(new { data = LaunchList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Launch.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Launch.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
