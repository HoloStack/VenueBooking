using Microsoft.AspNetCore.Mvc;

namespace venueBooking.Controllers
{
    public class HomeController : Controller
    {
        // GET: /
        // GET: /Home
        public IActionResult Index()
        {
            return View(); // will render Views/Home/Index.cshtml
        }

        // (optional) error page
        public IActionResult Error()
        {
            return View(); // render Views/Home/Error.cshtml
        }
    }
}
