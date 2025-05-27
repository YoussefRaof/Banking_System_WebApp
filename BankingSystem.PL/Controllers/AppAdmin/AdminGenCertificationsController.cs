using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.PL.Controllers.AppAdmin
{
    public class AdminGenCertificationsController : Controller
    {
        private readonly IGenericRepository<GeneralCertificate> _genericRepositoryCer;
        private readonly ISearchPaginationRepo<GeneralCertificate> _searchPaginationCer;


        public AdminGenCertificationsController(IGenericRepository<GeneralCertificate> genericRepositoryCer, ISearchPaginationRepo<GeneralCertificate> searchPaginationCer)
        {
            _genericRepositoryCer = genericRepositoryCer;
            _searchPaginationCer = searchPaginationCer;
        }


        // GET: AdminCertificationsController
        public ActionResult GetAllCertifications()
        {
            return View(_genericRepositoryCer.GetAll());
        }


        // GET: AdminCertificationsController/Details/5
        public ActionResult Details(int id)
        {
            return View(_genericRepositoryCer.Get(id));
        }


        // GET: AdminCertificationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminCertificationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GeneralCertificate certificate, string returnSection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _genericRepositoryCer.Add(certificate);
                    //TempData["ActiveSection"] = "certifications";

                    return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(certificate);
                }
            }
            return View(certificate);
        }


        // GET: AdminCertificationsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_genericRepositoryCer.Get(id));
        }

        // POST: AdminCertificationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GeneralCertificate certificate, string? returnSection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _genericRepositoryCer.Update(certificate);
                    return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(certificate);
                }
            }
            return View(certificate);
        }


        // POST: AdminCertificationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(GeneralCertificate certificate, string returnSection)
        {
            _genericRepositoryCer.Delete(certificate);
            return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
        }

    }
}
