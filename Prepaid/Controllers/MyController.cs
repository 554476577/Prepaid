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

    public class AdminController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Login() { return View(); }
    }
}