using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject.Infrastructure.Language;
using Portal.DAL;
using Portal.Models.ViewModels;

namespace Portal.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ProfileController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: Profile
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId).ToEnumerable();
            var profilesList = new List<ProfileView>();

            foreach (var profile in profiles)
            {
                profilesList.Add(new ProfileView
                {
                    Id = profile.Id,
                    Title = profile.Title,
                    Description = profile.Description,
                    IsDefault = profile.UserDataId == null
                });
            }

            return View(profilesList);
        }

        // GET: Profile/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Profile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Profile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Profile/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Profile/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
