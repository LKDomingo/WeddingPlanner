using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For Session
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity; // for Password Hasher
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;
using Microsoft.EntityFrameworkCore;
public class HomeController : Controller
{
    private MyContext _context;

    public HomeController( MyContext context )
    {
        _context = context;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Clear();

        /////////////////////////////
        // User? user = _context.Users.FirstOrDefault(a => a.Email == "dog@dog.com");
        // HttpContext.Session.SetInt32("User", user.UserId);
        // return RedirectToAction("Wedding");
        ////////////////////////////

        return View();
    }
    ////////////////REGISTER USER/////////////////
    [HttpPost("user/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(a => a.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
                return View("Index");
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

            _context.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("User", newUser.UserId);
            return RedirectToAction("DashBoard");
        }
        else
        {
            return View("Index");
        }
        
    }
    /////////////////LOGIN USER//////////////////
    [HttpPost("user/login")]
    public IActionResult Login(LogUser loginUser)
    {
        if (ModelState.IsValid)
        {
            // Grab the user from the Db
            User? userInDb = _context.Users.FirstOrDefault(a => a.Email == loginUser.LogEmail);

            // If user is not found in Db
            if (userInDb == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid login");
                return View("Index");
            }

            // Hash the LogUser password and check with the hashed password in the Db
            PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>();
            var result = Hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);

            // if result == 0, passwords did not match
            if (result == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid login");
                return View("Index");
            }

            HttpContext.Session.SetInt32("User", userInDb.UserId);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }
    //////////////////DASHBOARD//////////////////
    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        ViewBag.AllWeddings = _context.Weddings.Include(i => i.Guests).ThenInclude(i => i.User).ToList();
        // 
        return View();
    }
    //////////////////WEDDING FORM//////////////////
    [HttpGet("wedding")]
    public IActionResult Wedding()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        ViewBag.CreatorId = HttpContext.Session.GetInt32("User");
        return View();
    }
    ////////////////CREATE WEDDING/////////////////
    [HttpPost("create/wedding")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        if (ModelState.IsValid)
        {   
            // If wedding date is in the past
            if (newWedding.Date.Subtract(DateTime.Now).Hours < 0)
            {
                ModelState.AddModelError("Date", "Wedding date must be a future date");
                return View("Wedding");
            }

            // Add wedding to the Db
            _context.Add(newWedding);
            _context.SaveChanges();

            // Connect creator to the wedding
            // Association newAssociation = new Association();
            // Set associaton UserId and WeddingId 
            // newAssociation.UserId = (int)HttpContext.Session.GetInt32("User");
            // newAssociation.WeddingId = (int)newWedding.WeddingId;
            // _context.Add(newAssociation);
            // _context.SaveChanges();

            System.Console.WriteLine("Wedding Created!");

            return RedirectToAction("Dashboard");
        }
        else
        {
            ViewBag.CreatorId = HttpContext.Session.GetInt32("User");
            return View("Wedding");
        }
        
    }
    ///////////////////RSVP TO WEDDING/////////////////
    [HttpGet("RSVP/{WeddingId}")]
    public IActionResult RSVP(int WeddingId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        // Connect User to the wedding
            Association newAssociation = new Association();
            // Set associaton UserId and WeddingId 
            newAssociation.UserId = (int)HttpContext.Session.GetInt32("User");
            newAssociation.WeddingId = WeddingId;
            _context.Add(newAssociation);
            _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    ///////////////////UN-RSVP TO WEDDING/////////////////
    [HttpGet("UN-RSVP/{WeddingId}")]
    public IActionResult DeleteRSVP(int WeddingId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        Association? AssociationToDelete = _context.Associations.SingleOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User") && a.WeddingId == WeddingId);
            _context.Associations.Remove(AssociationToDelete);
            _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    ///////////////////DELETE WEDDING/////////////////
    [HttpGet("delete/{WeddingId}")]
    public IActionResult DeleteWedding(int WeddingId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            return Redirect("/");
        }
        Wedding? WeddingToDelete = _context.Weddings.FirstOrDefault(a =>  a.WeddingId == WeddingId);
            _context.Weddings.Remove(WeddingToDelete);
            _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
