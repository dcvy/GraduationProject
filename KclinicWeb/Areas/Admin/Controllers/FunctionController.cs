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
public class FunctionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public FunctionController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        FunctionVM FunctionVM = new()
        {
            Function = new(),
        };

        if (id == null || id == 0)
        {
            return View(FunctionVM);
        }
        else
        {
            FunctionVM.Function = _unitOfWork.Function.GetFirstOrDefault(u => u.Id == id);
            return View(FunctionVM);
        }


    }

	//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(FunctionVM obj, IFormFile? file)
	{
		if (ModelState.IsValid)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(wwwRootPath, @"images\functions");
				var extension = Path.GetExtension(file.FileName);

				if (obj.Function.ImageUrl != null)
				{
					var oldImagePath = Path.Combine(wwwRootPath, obj.Function.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStreams);
				}
				obj.Function.ImageUrl = @"\images\functions\" + fileName + extension;

			}
			if (obj.Function.Id == 0)
			{
				_unitOfWork.Function.Add(obj.Function);
			}
			else
			{
				_unitOfWork.Function.Update(obj.Function);
			}
			_unitOfWork.Save();
			TempData["success"] = "Function created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}



	#region API CALLS
	[HttpGet]
    public IActionResult GetAll()
    {
        var FunctionList = _unitOfWork.Function.GetAll();
        return Json(new { data = FunctionList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Function.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Function.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
