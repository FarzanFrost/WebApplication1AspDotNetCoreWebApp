using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
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
                }),

                ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
                { Text = i.Name,
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

                    // Since we are only using this only to get the details of the object, not to.
                    // perform any CRUD operations we dont track the object.
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extention = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                        {
                            Console.WriteLine("image save");
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extention;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(productVM.Product);
                }
                _db.SaveChanges();
                Console.WriteLine("product save");
                return RedirectToAction("Index"); 
            }
            productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0) return NotFound();
            Product product = _db.Product.Include(u => u.Category).Include(u=>u.ApplicationType).FirstOrDefault(u => u.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);
            if (obj == null) return NotFound();

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
