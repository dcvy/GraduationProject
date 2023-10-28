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
public class PartnerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public PartnerController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        PartnerVM PartnerVM = new()
        {
            Partner = new(),
        };

        if (id == null || id == 0)
        {
            return View(PartnerVM);
        }
        else
        {
            PartnerVM.Partner = _unitOfWork.Partner.GetFirstOrDefault(u => u.Id == id);
            return View(PartnerVM);
        }


    }

	//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(PartnerVM obj, IFormFile? file)
	{
		if (ModelState.IsValid)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(wwwRootPath, @"images\partners");
				var extension = Path.GetExtension(file.FileName);

				if (obj.Partner.ImageUrl != null)
				{
					var oldImagePath = Path.Combine(wwwRootPath, obj.Partner.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStreams);
				}
				obj.Partner.ImageUrl = @"\images\partners\" + fileName + extension;

			}
			if (obj.Partner.Id == 0)
			{
				_unitOfWork.Partner.Add(obj.Partner);
			}
			else
			{
				_unitOfWork.Partner.Update(obj.Partner);
			}
			_unitOfWork.Save();
			TempData["success"] = "Partner created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}



	#region API CALLS
	[HttpGet]
    public IActionResult GetAll()
    {
        var PartnerList = _unitOfWork.Partner.GetAll();
        return Json(new { data = PartnerList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Partner.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Partner.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
