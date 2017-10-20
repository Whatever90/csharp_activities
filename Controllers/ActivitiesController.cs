using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using connectingToDBTESTING.Models;
using System.Linq;
using Newtonsoft.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace connectingToDBTESTING.Controllers
{
    public class ActivitiesController : Controller
    {
        private ActivitiesContext _context;
 
        public ActivitiesController(ActivitiesContext context)
        {
            _context = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            
            List<Dictionary<string, object>> err = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("reg_errors"); 
            ViewBag.errors = err;
            string login_err = HttpContext.Session.GetObjectFromJson<string>("login_errors"); 
            ViewBag.err = login_err;
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")!=null){
                return RedirectToAction("Home");
            }
            return View();
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(User model)
        {
            if(ModelState.IsValid){
                User CurrentUser = new User(){
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    Email = model.Email,
                    ConPassword = model.ConPassword,
                };
                 _context.Add(CurrentUser);
                _context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("cur_user", CurrentUser);
                HttpContext.Session.SetObjectAsJson("reg_errors", null);
                HttpContext.Session.SetObjectAsJson("login_errors", null);
                return RedirectToAction("Home");
            }else{
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                Console.WriteLine(messages);
                HttpContext.Session.SetObjectAsJson("reg_errors", ModelState.Values);
                HttpContext.Session.SetObjectAsJson("login_errors", null);
                return RedirectToAction("Index");
            }
            
            //List<Dictionary<string, object>> Allq = _dbConnector.Query("SELECT * FROM quotes ORDER BY created_at Desc");
            
        }
        
        [HttpPost]
        [Route("/logining")]
        public IActionResult Logining(string email, string password)
        {
            User RUser = _context.Users.SingleOrDefault(user => user.Email == email);
            if(RUser==null){
                string errors = "Invalid email or password";
                HttpContext.Session.SetObjectAsJson("login_errors", errors); 
                return RedirectToAction("Index");
            }
            Console.WriteLine("++++++++++++++++++");
            Console.WriteLine(RUser.Password);
            Console.WriteLine(password);
            Console.WriteLine("++++++++++++++++++");
            if(RUser.Password==password){
                HttpContext.Session.SetObjectAsJson("cur_user", RUser);
                HttpContext.Session.SetObjectAsJson("login_errors", null);
                HttpContext.Session.SetObjectAsJson("reg_errors", null);
                return RedirectToAction("Home");
            }
            string errors2 = "Invalid email or password";
            HttpContext.Session.SetObjectAsJson("login_errors", errors2); 
            HttpContext.Session.SetObjectAsJson("reg_errors", null);
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            User RetrievedUser2 = _context.Users.SingleOrDefault(user => user.UserId == cur_user.UserId);
            List<Activity> filter = _context.Activities.Where(act => act.Date < DateTime.Now).ToList();
            foreach(Activity act in filter){
                Console.WriteLine(act.Name);
                _context.Activities.Remove(act);
                _context.SaveChanges();
            }
            List<Activity> AllA = _context.Activities.OrderBy(act => act.Date).Include(ac=>ac.User).Include(a=>a.Guests).ThenInclude(g => g.User).ToList();
            List<Guest> AllG = _context.Guests.ToList();
            ViewBag.AllGuests = AllG;
            ViewBag.cur_user = RetrievedUser2;
            ViewBag.AllA = AllA;
            string act_err = HttpContext.Session.GetObjectFromJson<string>("act_errors"); 
            ViewBag.act_err = act_err;
            HttpContext.Session.SetObjectAsJson("act_errors", null);
            HttpContext.Session.SetObjectAsJson("act2_errors", null);
            return View();
        }
        [HttpGet]
        [Route("createactivity")]
        public IActionResult createactivity()
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            List<Dictionary<string, object>> act_err = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("act_errors"); 
            ViewBag.act_errors = act_err;
            return View();
        }
        [HttpPost]
        [Route("newact")]
        public IActionResult newact(string Name, DateTime Date, int duration1, string duration2, string description)
        {   
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Console.WriteLine("+++++++++++++++++++++++++++++");
            DateTime endDate;
            if(duration2 == "min"){
                endDate = Date.AddMinutes(duration1);
            }else if(duration2 == "hours"){
                endDate = Date.AddHours(duration1);
            }else{
                endDate = Date.AddDays(duration1);
            }
            string Duration = duration1.ToString() + " " + duration2;
            Console.WriteLine(Name);
            Console.WriteLine(Date);
            Console.WriteLine(Duration);
            Console.WriteLine(endDate);
            Console.WriteLine(description);
            Console.WriteLine("+++++++++++++++++++++++++++++");
            //TryValidateModel(model);
            Activity act = new Activity(){
                    Name = Name,
                    Date = Date,
                    UserId = cur_user.UserId,
                    Duration = Duration,
                    End = endDate,
                    GuestsAmount = 0,
                    Description = description

                };
            
            TryValidateModel(act);
            if(ModelState.IsValid){
                
                _context.Add(act);
                _context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("act_errors", null);
                return RedirectToAction("Home");
            }else{
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                Console.WriteLine(messages);
                HttpContext.Session.SetObjectAsJson("act_errors", ModelState.Values);
                return RedirectToAction("createactivity");
            
        }
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult delete(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            if(RA.UserId==cur_user.UserId){
                _context.Activities.Remove(RA);
                _context.SaveChanges();
            }else{
                Console.WriteLine("WARNING! HACKER IS DETECTED! SEARCH AND DESTROY!");
            }
            return RedirectToAction("Home");
        }
        [HttpGet]
        [Route("destroy/{id}")]
        public IActionResult destroy(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            if(RA.UserId==cur_user.UserId){
                _context.Activities.Remove(RA);
                _context.SaveChanges();
            }else{
                Console.WriteLine("WARNING! HACKER IS DETECTED! SEARCH AND DESTROY!");
            }
            return RedirectToAction("Activity", new{id = id});
        }
        [HttpGet]
        [Route("attend/{id}")]
        public IActionResult attend(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            List<Activity> AllA = _context.Activities.OrderBy(act => act.Date).Include(ac=>ac.User).Include(a=>a.Guests).ThenInclude(g => g.User).ToList();
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            int check = 0;
            foreach(Activity act in AllA){
                foreach(Guest g in act.Guests){
                    if(g.UserId == cur_user.UserId){
                        if(RA.Date<act.End && RA.End > act.Date){
                            check = 1;
                        } else if(RA.Date>act.Date && RA.Date < act.End){
                            check = 1;
                        } else if(RA.End>act.Date && RA.End < act.End){
                            check = 1;
                        }
                    }
                }
            }
            if(check == 0){
                RA.GuestsAmount++;
                _context.SaveChanges();
                Guest NewGuest = new Guest{
                    UserId = cur_user.UserId,
                    ActivityId = id
                };
                _context.Add(NewGuest);
                _context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("act_errors", null);
                return RedirectToAction("Home");
            }else{
                string errors2 = "Oops, You can't go there";
                HttpContext.Session.SetObjectAsJson("act_errors", errors2); 
                return RedirectToAction("Home");
            }
            
        }
        [HttpGet]
        [Route("join/{id}")]
        public IActionResult Join(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            List<Activity> AllA = _context.Activities.OrderBy(act => act.Date).Include(ac=>ac.User).Include(a=>a.Guests).ThenInclude(g => g.User).ToList();
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            int check = 0;
            foreach(Activity act in AllA){
                foreach(Guest g in act.Guests){
                    if(g.UserId == cur_user.UserId){
                        if(RA.Date<act.End && RA.End > act.Date){
                            check = 1;
                        } else if(RA.Date>act.Date && RA.Date < act.End){
                            check = 1;
                        } else if(RA.End>act.Date && RA.End < act.End){
                            check = 1;
                        }
                    }
                }
            }
            if(check == 0){
                RA.GuestsAmount++;
                _context.SaveChanges();
                Guest NewGuest = new Guest{
                    UserId = cur_user.UserId,
                    ActivityId = id
                };
                _context.Add(NewGuest);
                _context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("act2_errors", null);
                return RedirectToAction("Activity", new{id = id});
            }else{
                string errors2 = "Oops, You can't go there";
                HttpContext.Session.SetObjectAsJson("act2_errors", errors2); 
                return RedirectToAction("Activity", new{id = id});
            }
            
        }
        [HttpGet]
        [Route("changeyourmind/{id}")]
        public IActionResult changeyourmind(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Guest RetrievedGuest = _context.Guests.Where(act => act.ActivityId == id).SingleOrDefault(user=> user.UserId == cur_user.UserId);
            _context.Guests.Remove(RetrievedGuest);
            _context.SaveChanges();
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            RA.GuestsAmount--;
            _context.SaveChanges();
            
            return RedirectToAction("Home");
        }
        [HttpGet]
        [Route("cancel/{id}")]
        public IActionResult cancel(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Guest RetrievedGuest = _context.Guests.Where(act => act.ActivityId == id).SingleOrDefault(user=> user.UserId == cur_user.UserId);
            _context.Guests.Remove(RetrievedGuest);
            _context.SaveChanges();
            Activity RA = _context.Activities.SingleOrDefault(act => act.ActivityId == id);
            RA.GuestsAmount--;
            _context.SaveChanges();
            
            return RedirectToAction("Activity", new{id = id});
        }
        [HttpGet]
        [Route("activity/{id}")]
        public IActionResult Activity(int id)
        {
            if(HttpContext.Session.GetObjectFromJson<User>("cur_user")==null){
                return RedirectToAction("Index");
            }
            Console.WriteLine(id);
            User cur_user = HttpContext.Session.GetObjectFromJson<User>("cur_user");
            Activity RA = _context.Activities.Include(ac=>ac.User).Include(a=>a.Guests).ThenInclude(g => g.User).SingleOrDefault(act => act.ActivityId == id);
            ViewBag.act = RA;
            ViewBag.cur_user = cur_user;
            string act2_err = HttpContext.Session.GetObjectFromJson<string>("act2_errors"); 
            ViewBag.act2_err = act2_err;
            return View();
        }

            //     _bcontext.Add(tr);
            //     _bcontext.SaveChanges();
            //     User RetrievedUser = _bcontext.Users.SingleOrDefault(user => user.UserId == cur_user.UserId);
            //     Console.WriteLine("++++++++++++++++++");
            //     Console.WriteLine(RetrievedUser.Balance + model.Activity);
            //     RetrievedUser.Balance = RetrievedUser.Balance + model.Activity;
            //     Console.WriteLine(RetrievedUser.Balance);
            //     _bcontext.SaveChanges();
            
            
            // Console.WriteLine("++++++++++++++++++");
            
        

        // [HttpPost]
        // [Route("/addrev")]
        // public IActionResult adduser(Review NewRev)
        // {
            
        //     if(ModelState.IsValid){
        //         _context.Add(NewRev);
        //         HttpContext.Session.SetObjectAsJson("TheList", null);
        //     // OR _context.Users.Add(NewPerson);
        //         _context.SaveChanges();
        //         return RedirectToAction("allTransactions");
        //     }else{
        //         string messages = string.Join("; ", ModelState.Values
        //                                 .SelectMany(x => x.Errors)
        //                                 .Select(x => x.ErrorMessage));
        //         Console.WriteLine(messages);
        //         HttpContext.Session.SetObjectAsJson("TheList", ModelState.Values);
        //         return RedirectToAction("Index");
        //     }
            
        // }
        // [HttpGet]
        // [Route("account/{id}")]
        // public IActionResult allTransactions()
        // {
        //     List<Transaction> Transactions = _tcontext.Transactions.Include(tran => tran.User).OrderByDescending(tran => tran.CreatedAt).ToList();
        //     ViewBag.Transactions = Transactions;
        //     return View();
        // }
        [HttpGet]
        [Route("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.SetObjectAsJson("cur_user", null);
            return RedirectToAction("Index");
        }
            
    }


public static class SessionExtensions
{
    // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        // This helper function simply serializes theobject to JSON and stores it as a string in session
        session.SetString(key, JsonConvert.SerializeObject(value));
    }
       
    // generic type T is a stand-in indicating that we need to specify the type on retrieval
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        // Upon retrieval the object is deserialized based on the type we specified
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
}
