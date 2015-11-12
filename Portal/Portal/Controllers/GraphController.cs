using System.Web.Mvc;
using Portal.DAL;

namespace Portal.Controllers
{
    [Authorize]
    public class GraphController : Controller
    {
        private readonly IUnitOfWork _uow;
        public GraphController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }
    }
}