using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public RoleManagmentVM Input { get; set; }

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
		}

		public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string id)
        {
            ApplicationUser objFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id, includeProperties:"Company");
            if (objFromDb == null)
            {
                return RedirectToAction(nameof(Index));
            }

            Input = new()
            {
                User = objFromDb,
                RoleList = _roleManager.Roles.Select(u => u.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            Input.User.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id)).GetAwaiter().GetResult().FirstOrDefault();
            return View(Input);
        }

        [HttpPost]
        public IActionResult RoleManagment()
        {
            if(ModelState.IsValid)
            {
                string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == Input.User.Id))
                        .GetAwaiter().GetResult().FirstOrDefault();

                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == Input.User.Id);


                if (!(Input.User.Role == oldRole))
                {
                    //a role was updated
                    if (Input.User.Role == SD.Role_Company)
                    {
                        applicationUser.CompanyId = Input.User.CompanyId;
                    }
                    if (oldRole == SD.Role_Company)
                    {
                        applicationUser.CompanyId = null;
                    }
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();

                    _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(applicationUser, Input.User.Role).GetAwaiter().GetResult();

                }
                else
                {
                    if (oldRole == SD.Role_Company && applicationUser.CompanyId != Input.User.CompanyId)
                    {
                        applicationUser.CompanyId = Input.User.CompanyId;
                        _unitOfWork.ApplicationUser.Update(applicationUser);
                        _unitOfWork.Save();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objApplicationUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();
            foreach(ApplicationUser user in objApplicationUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if(user.Company == null)
                {
                    user.Company = new Company() { Name = ""};
                }
            }
            return Json(new { data = objApplicationUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Did not found the user to lock or unlock." });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked: unlocking...
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "User lock was changed." });
        }
        #endregion
    }
}
