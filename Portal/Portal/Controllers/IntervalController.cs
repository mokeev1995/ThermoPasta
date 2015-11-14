using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Portal.DAL;
using Portal.Models.CodeFirstModels;
using Portal.Models.ViewModels;

namespace Portal.Controllers
{
    public class IntervalController : BaseController
	{
        private readonly IUnitOfWork _uow;
        public IntervalController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
        // GET: Interval
        public async Task<ActionResult> Index(int id)
        {
            ViewBag.ProfileId = id;
            var intervals = _uow.IntervalRepository.GetAll().Where(i => i.ProfileId == id).OrderBy(i => i.Start);
            var intervalViews = new List<IntervalView>();
            foreach (var interval in intervals)
            {
                intervalViews.Add(new IntervalView
                {
                    Id = interval.Id,
                    Start = interval.Start,
                    End = interval.End,
                    Description = interval.Description
                });
            }

            return PartialView(intervalViews);
        }

        [ChildActionOnly]
        public ActionResult List(int id)
        {
            ViewBag.ProfileId = id;
            var intervals = _uow.IntervalRepository.GetAll().Where(i => i.ProfileId == id).OrderBy(i => i.Start);
            var intervalViews = new List<IntervalView>();
            foreach (var interval in intervals)
            {
                intervalViews.Add(new IntervalView
                {
                    Id = interval.Id,
                    Start = interval.Start,
                    End = interval.End,
                    Description = interval.Description
                });
            }

            return PartialView(intervalViews);
        }

        public ActionResult Create(int profileId)
        {
            var intervalCreate = new IntervalCreate
            {
                ProfileId = profileId
            };
            return PartialView(intervalCreate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IntervalCreate intervalView)
        {
            if (ModelState.IsValid)
            {
                var interval = new Interval
                {
                    Description = intervalView.Description,
                    End = intervalView.End,
                    Start = intervalView.Start,
                    ProfileId = intervalView.ProfileId
                };

                _uow.IntervalRepository.Insert(interval);
                _uow.Save();

                string url = Url.Action("Index", "Interval", new { id = intervalView.ProfileId });
                return Json(new { success = true, url = url });
            }

            return PartialView(intervalView);
        }


        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var interval = _uow.IntervalRepository.GetById(id);

            if (interval == null)
            {
                return HttpNotFound();
            }

            var intervalVirew = new IntervalCreate
                {
                    Start = interval.Start,
                    End = interval.End,
                    Description = interval.Description,
                    Id = interval.Id,
                    ProfileId = interval.ProfileId
                };

            return PartialView(intervalVirew);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Start,End,Description")] IntervalCreate intervalCreate)
        {
            if (ModelState.IsValid)
            {
                var interval = new Interval
                {
                    Description = intervalCreate.Description,
                    End = intervalCreate.End,
                    Start = intervalCreate.Start,
                    ProfileId = intervalCreate.ProfileId
                };

                _uow.IntervalRepository.Update(interval);
                _uow.Save();

                string url = Url.Action("Index", "Interval", new { id = intervalCreate.ProfileId });
                return Json(new { success = true, url = url });
            }
            return PartialView(intervalCreate);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var interval = _uow.IntervalRepository.GetById(id);
            if (interval == null)
            {
                return HttpNotFound();
            }
            var intervalVirew = new IntervalView
            {
                Start = interval.Start,
                End = interval.End,
                Description = interval.Description,
                Id = interval.Id,
            };

            return PartialView(intervalVirew);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var interval = _uow.IntervalRepository.GetById(id);
            _uow.IntervalRepository.Delete(interval);
            _uow.Save();

            string url = Url.Action("Index", "Interval", new { id = interval.ProfileId });
            return Json(new { success = true, url = url });
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
