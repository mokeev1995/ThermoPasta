using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.DAL;
using Portal.Helpers;

namespace Portal.Controllers
{
    public class HomeController : BaseController
	{
        private readonly IUnitOfWork _uow;

        public HomeController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
			SetUserName(_uow);
            string h2 = "Hello, ";
            object h4 = "Now you ";

	        var emptyName = "Guest";

			if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var userData = _uow.UserDataRepository.GetById(userId);
                if (userData == null)
                {
                    h2 += emptyName;
                    h4 = "We don't know who you are, bacause information about you doesn't exist, but you successfully registered. Have fun.";
                }
                else
                {
                    h2 += string.Format("{0} {1}", userData.FirstName, userData.LastName);
	                ViewBag.UserName = userData.FirstName + " " + userData.LastName;
                    var deviceCount = userData.UserDevices.Count;
                    if (deviceCount > 0)
                    {
                        h4 += string.Format("have {0} device(s)", deviceCount);
                    }
                    else
                    {
                        h4 += "can add new device";
                    }
                }
            }
            else
            {
				h2 += emptyName;
                h4 = new HtmlString(RenderPartialViewToString("_NotAuthed", null));
            }

            ViewBag.H2 = h2;
            ViewBag.H4 = h4;
            return View();
        }

		protected string RenderPartialViewToString(string viewName, object model)
		{
			if (string.IsNullOrEmpty(viewName))
				viewName = ControllerContext.RouteData.GetRequiredString("action");

			ViewData.Model = model;

			using (StringWriter sw = new StringWriter())
			{
				ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}
		}
	}
}