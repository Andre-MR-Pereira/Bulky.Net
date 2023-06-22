using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
        {
			List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

		public IActionResult Upsert(int? id)
		{
			if(id == null || id == 0)
			{
				return View(new Company());
			}
			Company company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
			return View(company);
		}

		[HttpPost]
		public IActionResult Upsert(Company company)
		{
			if(ModelState.IsValid)
			{
				if(company.Id == 0 )
				{
					TempData["success"] = "Company has been created successfully!";
					_unitOfWork.Company.Add(company);
				}
				else
				{
					TempData["success"] = "Company has been updated successfully!";
					_unitOfWork.Company.Update(company);
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{
				return View(company = new Company());
			}
		}

		public IActionResult Delete(int? id) 
		{ 
			if( id == null || id == 0)
			{
				return NotFound();
			}
			else
			{
				Company company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
				return View(company);
			}
		}

		[HttpPost]
		public IActionResult Delete(Company company)
		{
			if (ModelState.IsValid && company.Id > 0)
			{
				TempData["success"] = "Company has been deleted successfully!";
				_unitOfWork.Company.Delete(company);
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Company was not found!";
				return NotFound();
			}
		}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        #endregion
    }
}
