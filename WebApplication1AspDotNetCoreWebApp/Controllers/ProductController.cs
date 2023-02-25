using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using WebApplication1AspDotNetCoreWebApp.Data;
using WebApplication1AspDotNetCoreWebApp.Models;
using WebApplication1AspDotNetCoreWebApp.Models.ViewModels;

namespace WebApplication1AspDotNetCoreWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product;

            foreach(var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            };
            return View(objList);
        }

        // Instead of using this method we can use view models, to make use to strongly typed views, so the below functions uses it.
        //GET - Upsert
        //public IActionResult Upsert(int? id)
        //{
        //    // This is Like a dto that can be generated with out defining explicitly.
        //    // Here we need the name and id of each category to use in the drop down menu.
        //    // Here inside select we define the dto.
        //    IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
        //    {
        //        Text= i.Name,
        //        Value = i.Id.ToString(),
        //    });

        //    // To pass the variables to the view we use a view bag.
        //    ViewBag.CategoryDropDown = CategoryDropDown;

        //    Product product = new Product();
        //    if(id != null)
        //    {
        //        product = _db.Product.Find(id);
        //        if(product == null) { return NotFound(); }
        //    }
        //    return View(product);
        //}

        public IActionResult Upsert(int? id)
        {
            Console.WriteLine("before PVM");
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            Console.WriteLine("after PVM");

            //if (id != null)
            //{
            //    productVM.Product = _db.Product.Find(id);
            //    if (productVM.Product == null) { return NotFound(); }
            //}
            if (id == null)
            {
                Console.WriteLine("when id == null");
                return View(productVM);
            }
            else
            {
                Console.WriteLine("when id != null");
                productVM.Product = _db.Product.Find(id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        [HttpPost] 
        [ValidateAntiForgeryToken] 
        public IActionResult Upsert(ProductVM productVM)
        {

            Console.WriteLine("Post request - FARZAN");
            // FIXME: Need to recheck the validation as the ModelState is invalid.
            if (!ModelState.IsValid) 
            {
                var files = HttpContext.Request.Form.Files;
                // This is the path to wwwroot.
                // We take this from the existing dependency in the framework.
                string webRootPath = _webHostEnvironment.WebRootPath;
                Console.WriteLine("Post - before product Id == null");
                if (productVM.Product.Id== 0)
                {
                    Console.WriteLine("Post - after product Id == null");

                    //Creating a product

                    string upload = webRootPath + WC.ImagePath;
                    // Here we generate a random guid for the file name.
                    string fileName = Guid.NewGuid().ToString();
                    string extention = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                    {
                        Console.WriteLine("image save");
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extention;
                    _db.Product.Add(productVM.Product);
                    Console.WriteLine("product add");

                }
                else
                {
                    //updating a product
                }
                _db.SaveChanges();
                Console.WriteLine("product save");
                return RedirectToAction("Index"); 
            }
            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var obj = _db.Product.Find(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);
            if (obj == null) return NotFound();
            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
