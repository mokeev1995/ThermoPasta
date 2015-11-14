using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject.Infrastructure.Language;
using Portal.DAL;
using Portal.Models.CodeFirstModels;
using Portal.Models.ViewModels;

namespace Portal.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
	{
        private readonly IUnitOfWork _uow;

        public ProfileController(IUnitOfWork uow) :base(uow)
        {
            _uow = uow;
        }

        // GET: Profile
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var profiles =
                _uow.ProfileRepository.GetAll()
                    .Where(p => p.UserDataId == null || p.UserDataId == userId)
                    .ToEnumerable();
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
            var profile = _uow.ProfileRepository.GetAll().First(p => p.Id == id);

            var intervalsList = new List<IntervalView>();
            var intervals = profile.Intervals.OrderBy(i => i.Start);

            foreach (var interval in intervals)
            {
                intervalsList.Add(
                    new IntervalView
                    {
                        Description = interval.Description,
                        Start = interval.Start,
                        End = interval.End
                    });
            }
            var profileView = new ProfileView
            {
                Id = profile.Id,
                Title = profile.Title,
                Description = profile.Description,
                IsDefault = profile.UserDataId == null,
                Intervals = intervalsList,
                Type = profile.UserDataId == null ? "Default" : "Custom"
            };

            return View(profileView);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProfileCreate profileCreate)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var profile = new Profile
                {
                    Description = profileCreate.Description,
                    Title = profileCreate.Title,
                    UserDataId = userId
                };

                _uow.ProfileRepository.Insert(profile);
                _uow.Save();

                return RedirectToAction("Index");
            }

            return View(profileCreate);
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var profile = _uow.ProfileRepository.GetById(id);


            if (profile == null)
            {
                return HttpNotFound();
            }

            var profileView = new ProfileCreate
            {
                Id = profile.Id,
                Title = profile.Title,
                Description = profile.Description,
            };
            return View(profileView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProfileCreate profileCreate)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var profile = new Profile
                {
                    Id = profileCreate.Id,
                    Description = profileCreate.Description,
                    Title = profileCreate.Title,
                    UserDataId = userId
                };

                _uow.ProfileRepository.Update(profile);
                _uow.Save();

                return RedirectToAction("Index");
            }
            return View(profileCreate);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var profile = _uow.ProfileRepository.GetById(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            var profileView = new ProfileView
            {
                Id = profile.Id,
                Title = profile.Title,
                Description = profile.Description,
            };

            return View(profileView);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           _uow.ProfileRepository.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
               _uow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}