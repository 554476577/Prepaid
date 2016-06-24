using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Prepaid.Controllers
{
    public class HomeController : Controller
    {
        // 系统首页
        public ActionResult Index() { return View(); }
    }
}