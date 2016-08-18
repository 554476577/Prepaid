using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Prepaid.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() { return View(); }
    }

    public class AdminController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Login() { return View(); }
    }

    public class PointController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class CreditLevelController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class RechargeController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }
    }

    public class UserController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class DeviceLinkController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class EnergyBillController : Controller
    {
        public ActionResult List() { return View(); }
    }

    public class PrepaidController : Controller
    {
        public ActionResult List() { return View(); }
    }

    public class CommunityController : Controller 
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }
}