using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.Controllers
{
    public class ErrorDialogueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginError()
        {
            return View();
        }
        public IActionResult SearchError()
        {
            return View();
        }
    }
}
