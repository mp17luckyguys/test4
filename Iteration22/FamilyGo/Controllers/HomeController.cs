using FamilyGo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FamilyGo.Utils.ExcelToDBActivity;
using static FamilyGo.Utils.ExcelToDBOtherActivity;
using static FamilyGo.Utils.ExcelToDBParks;
using static FamilyGo.Utils.ExcelToDBSportPlace;

namespace FamilyGo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            //ExcelToDB ex = new ExcelToDB();
            //ex.Page_Load();
            //ExcelToDBPark ex1 = new ExcelToDBPark();
            //ex1.Page_Load();
            //ExcelToDB ex = new ExcelToDB();
            //ex.Page_Load();
            //ExcelToDBOtherActicityPlaces ex = new ExcelToDBOtherActicityPlaces();
            //ex.Page_Load();
            //ExcelToDBSportPlaces ex2 = new ExcelToDBSportPlaces();
            //ex2.Page_Load();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}