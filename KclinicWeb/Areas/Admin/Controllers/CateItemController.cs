using Kclinic.DataAccess;
using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using Kclinic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KclinicWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CateItemController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CateItemController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<CateItem> objCateItemList = _unitOfWork.CateItem.GetAll();
        return View(objCateItemList);
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CateItem obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CateItem.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "CateItem created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        //var cateItemFromDb = _db.CateItems.Find(id);
        var cateItemFromDbFirst = _unitOfWork.CateItem.GetFirstOrDefault(u => u.Id == id);
        //var cateItemFromDbSingle = _db.CateItems.SingleOrDefault(u => u.Id == id);

        if (cateItemFromDbFirst == null)
        {
            return NotFound();
        }

        return View(cateItemFromDbFirst);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CateItem obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CateItem.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "CateItem updated successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        //var cateItemFromDb = _db.Cateitems.Find(id);
        var cateItemFromDbFirst = _unitOfWork.CateItem.GetFirstOrDefault(u => u.Id == id);
        //var cateItemFromDbSingle = _db.CateItems.SingleOrDefault(u => u.Id == id);

        if (cateItemFromDbFirst == null)
        {
            return NotFound();
        }

        return View(cateItemFromDbFirst);
    }

    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
        var obj = _unitOfWork.CateItem.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }

        _unitOfWork.CateItem.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "CateItem deleted successfully";
        return RedirectToAction("Index");

    }
}
