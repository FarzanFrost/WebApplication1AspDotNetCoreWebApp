using Microsoft.AspNetCore.Mvc;
using WebApplication1AspDotNetCoreWebApp.Data;
using WebApplication1AspDotNetCoreWebApp.Models;

namespace WebApplication1AspDotNetCoreWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Since the ApplicationDbContext is added in Program.cs, this is called as a dependency injection
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            return View(objList);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE, here obj is the object that we have to add to the database
        [HttpPost] /*to say that this is a post request*/
        [ValidateAntiForgeryToken] /*A build in mechanism */
        public IActionResult Create(Category obj)
        {
            if(ModelState.IsValid) /*here this checks if all validations are met*/
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); /*Since the redirect page is in the same controller we don't have to mention the controller*/
            }
            return View(obj); /*if validations are not met here the user is directed to the view, with the previous values, through obj object*/
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var obj = _db.Category.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var obj = _db.Category.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Category.Find(id);
            if (obj == null) return NotFound();
            _db.Category.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
