using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StoryWriter.Controllers
{
    /// <summary>
    /// This is a simple one-page controller which simply forwards the user to our index view.
    /// </summary>
    public class HomeController : Controller
    {

        /// <summary>
        /// The index page for the application - just display the homepage.
        /// </summary>
        /// <returns>
        /// The index view result.
        /// </returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}