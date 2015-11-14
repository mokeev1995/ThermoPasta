using System.Web.Mvc;
using Portal.DAL;
using Portal.Helpers;

namespace Portal.Controllers
{
	public class BaseController : Controller
	{
		public BaseController(IUnitOfWork uow)
		{
			SetUserName(uow);
		}

		protected void SetUserName(IUnitOfWork uow)
		{
			Helper.SetUsername(User, uow, ViewBag);
		}
	}
}