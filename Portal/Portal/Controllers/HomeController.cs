using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.DAL;

namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uow;

        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            string h2 = "Hello, ";
            string h4 = "Now you ";
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var userData = _uow.UserDataRepository.GetById(userId);
                if (userData == null)
                {
                    h2 += "Foo Bar";
                    h4 = "We don't know who you are, bacause information about you doesn't exist, but you successfully registered. Have fun.";
                }
                else
                {
                    h2 += string.Format("{0} {1}", userData.FirstName, userData.LastName);
                    var deviceCount = userData.Devices.Count;
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
                h2 += "Foo Bar";
                h4 += "can sign in or sign up";
            }

            ViewBag.H2 = h2;
            ViewBag.H4 = h4;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}