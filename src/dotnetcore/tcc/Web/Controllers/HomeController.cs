using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Core.Data;
using Core.Cache;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Constants;
using System.Linq;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICacher _cacher;
        private readonly IRepository<ProductModel, int> _repository;
        public HomeController(
            ICacher cacher,
            IRepository<ProductModel, int> repository)
        {
            _cacher = cacher;
            _repository = repository;
        }
        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _cacher.GetAsync<Dictionary<int, ProductModel>>(GlobalConstants.CACHE_KEY_PRODUCT_MODELS).ConfigureAwait(false);
            return View(response.Values.Skip(0).Take(10));
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
