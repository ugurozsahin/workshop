using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Core.Data;
using System.Threading.Tasks;
using Core.Cache;
using System;
using Core.Constants;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ICacher _cacher;
        private readonly IRepository<ProductModel, int> _repository;
        public ProductController(
            ICacher cacher,
            IRepository<ProductModel, int> repository)
        {
            _cacher = cacher;
            _repository = repository;
        }
        // GET: api/product
        [HttpGet]
        public async Task<IEnumerable<ProductModel>> Get()
        {
            var response = await _cacher.GetAsync<Dictionary<int, ProductModel>>(GlobalConstants.CACHE_KEY_PRODUCT_MODELS).ConfigureAwait(false);
            return response.Values;
        }

        // GET api/product/1
        [HttpGet("{id}")]
        public async Task<ProductModel> Get(int id)
        {
            var response = await _repository.GetAsync(id).ConfigureAwait(false);
            return response;
        }

        // POST api/product
        [HttpPost]
        public async Task Post([FromBody]ProductModel productModel)
        {
            await _repository.CreateAsync(productModel).ConfigureAwait(false);
        }

        // PUT api/product/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]ProductModel productModel)
        {
            productModel.Id = id;
            productModel.LastUpdatedTime = DateTime.Now;
            await _repository.UpdateAsync(productModel).ConfigureAwait(false);
        }

        // DELETE api/product/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _repository.DeleteAsync(id).ConfigureAwait(false);
        }
    }
}
