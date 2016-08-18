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

    public class BasicController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class AdminController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Login() { return View(); }
    }

    public class DeviceTypeController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class CreditController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class LadderController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class CommunityController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class BuildingController : Controller
    {
        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }
    }

    public class RoomController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class DeviceController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }
    }

    public class BillController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Prepaid() { return View(); }
    }

    public class RechargeController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }
    }

    public class NoticeController : Controller
    {
        public ActionResult List() { return View(); }
    }
}