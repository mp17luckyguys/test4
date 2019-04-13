using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FamilyGo.Models;
using FamilyGo.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyGo.Controllers
{
    public class PlacesController : Controller
    {
        private FamilyGoiteration1_dbEntities db = new FamilyGoiteration1_dbEntities();

        // GET: Places
        /*       public ActionResult Index()
               {
                   var places = db.Places.Include(p => p.Activity);
                               List<Place> placesList = places.ToList();
                               List<Place> newPlacesList = new List<Place>();
                               foreach (Place place in placesList)
                               {
                                   if (place.ActivityActivityId == i)
                                       newPlacesList.Add(place);
                               }

                               return View(newPlacesList);
                   return View(places.ToList());
               }*/

        public ActionResult Index(string i)
        {
            var places = db.Places.Include(p => p.Activity);
            List<Place> placesList = places.ToList();
            List<Place> newPlacesList = new List<Place>();
            foreach (Place place in placesList)
            {
                if (string.Equals(place.Activity.Name,i))
                    newPlacesList.Add(place);
            }
            /*            List < Activity > acti= db.Activities.ToList();
                        Dictionary<string, string> DATA1 = new Dictionary<string, string>();
                        Dictionary<string, string> DATA2 = new Dictionary<string, string>();
                        Dictionary<string, string> DATA3 = new Dictionary<string, string>();
                        foreach (Activity a in acti)
                        {
                            DATA1.Add(a.Name, a.Name);
                            if (a.Description == "12" || a.Description == "1") { DATA2.Add(a.Name, a.Name); }
                            if (a.Description == "12" || a.Description == "2") { DATA3.Add(a.Name, a.Name); }
                        }
                        DATA1.Add("X", "X");
                        DATA2.Add("X", "X");
                        DATA3.Add("X", "X");
                        ViewBag.Data1 = DATA1;
                        ViewBag.Data2 = DATA2;
                        ViewBag.Data3 = DATA3;*/
            ViewBag.activityName = i;
            return View(newPlacesList);
        }

        // GET: Places/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            string googleTextSearchReasult = HttpUtils.Get("https://maps.googleapis.com/maps/api/place/textsearch/json?query="+ place.Name + "&key=AIzaSyDJeTABC7AwjSI-x7dm2cVlbHvA3yN65HA");
            JObject jo = (JObject)JsonConvert.DeserializeObject(googleTextSearchReasult);
            if (jo["status"].ToString() != "ZERO_RESULTS")
            {
                string placeId = jo["results"][0]["place_id"].ToString(); 
                if (jo["results"][0]["photos"] != null)
                {
                    
                    string joPhotoRef = jo["results"][0]["photos"][0]["photo_reference"].ToString();

                    //Image googlePhoto = HttpUtils.GetImage("https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + joPhotoRef + "& sensor=false&key=AIzaSyDJeTABC7AwjSI-x7dm2cVlbHvA3yN65HA");
                    string imageUrl = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + joPhotoRef + "& sensor=false&key=AIzaSyDJeTABC7AwjSI-x7dm2cVlbHvA3yN65HA";
                    ViewBag.imUrl = imageUrl;

                }
                string detailUrl = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "& fields=name,rating,formatted_phone_number,reviews,website,opening_hours&key=AIzaSyDJeTABC7AwjSI-x7dm2cVlbHvA3yN65HA";
                string googleDetailReasult = HttpUtils.Get(detailUrl);
                JObject joDetail = (JObject)JsonConvert.DeserializeObject(googleDetailReasult);
                if (joDetail["result"]["rating"] != null)
                { ViewBag.rating = joDetail["result"]["rating"]; }
                else { ViewBag.rating = "No rating avaliable."; }
                if (joDetail["result"]["formatted_address"] != null) { ViewBag.address = joDetail["result"]["formatted_address"]; } else { ViewBag.address = "No address avaliable."; }
                if (joDetail["result"]["formatted_phone_number"] != null) { ViewBag.phoneNumber = joDetail["result"]["formatted_phone_number"]; } else { ViewBag.phoneNumber = "No phone number avaliable."; }
                if (joDetail["result"]["website"] != null) { ViewBag.website = joDetail["result"]["website"]; } else { ViewBag.website = "No website avaliable."; }
                if (joDetail["result"]["opening_hours"] != null) { if (joDetail["result"]["opening_hours"]["open_now"].ToString() == "true") { ViewBag.openingNow = "Opening"; } else { ViewBag.openingNow = "Closed"; } } else { ViewBag.openingNow = "Unknown"; }
                if (joDetail["result"]["opening_hours"] != null) { ViewBag.weekday = joDetail["result"]["opening_hours"]["weekday_text"]; } else { ViewBag.weekday = "No weekday information avaliable."; }
                if (joDetail["result"]["reviews"] != null) { ViewBag.review = joDetail["result"]["reviews"]; } else { ViewBag.review = "No review avaliable."; }
            }
            else { ViewBag.imUrl = "../../Image/noPhoto.png"; ViewBag.openingNow = "Unknown"; ViewBag.website = "No website avaliable."; ViewBag.phoneNumber = "No phone number avaliable."; ViewBag.rating = "No rating avaliable."; ViewBag.weekday = "No opening hours avaliable."; ViewBag.review = "No review avaliable."; }

            return View(place);
        }

        // GET: Places/Create
        public ActionResult Create()
        {
            ViewBag.ActivityActivityId = new SelectList(db.Activities, "ActivityId", "Name");
            return View();
        }

        // POST: Places/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlaceId,Name,Address,Facility,Lat,Lon,Suburb,ActivityActivityId")] Place place)
        {
            if (ModelState.IsValid)
            {
                db.Places.Add(place);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityActivityId = new SelectList(db.Activities, "ActivityId", "Name", place.ActivityActivityId);
            return View(place);
        }

        // GET: Places/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityActivityId = new SelectList(db.Activities, "ActivityId", "Name", place.ActivityActivityId);
            return View(place);
        }

        // POST: Places/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlaceId,Name,Address,Facility,Lat,Lon,Suburb,ActivityActivityId")] Place place)
        {
            if (ModelState.IsValid)
            {
                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityActivityId = new SelectList(db.Activities, "ActivityId", "Name", place.ActivityActivityId);
            return View(place);
        }

        // GET: Places/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Place place = db.Places.Find(id);
            db.Places.Remove(place);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
