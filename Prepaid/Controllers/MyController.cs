﻿using System;
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

        public ActionResult Setting() { return View(); }
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

        public ActionResult Bills() { return View(); }

        public ActionResult Recharges() { return View(); }

        public ActionResult BatchImport() { return View(); }
    }

    public class DeviceController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Add() { return View(); }

        public ActionResult Modify() { return View(); }

        public ActionResult Details() { return View(); }

        public ActionResult BatchImport() { return View(); }

        public ActionResult SimpleAdd() { return View(); }
    }

    public class BillController : Controller
    {
        public ActionResult List() { return View(); }

        public ActionResult Cube() { return View(); }

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

        public ActionResult Alarm() { return View(); }
    }

    public class LoggerController : Controller
    {
        public ActionResult List() { return View(); }
    }
}