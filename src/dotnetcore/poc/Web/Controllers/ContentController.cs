using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Core.Data;
using System.Threading.Tasks;
using Data.RabbitMq;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
    {
        private readonly IRepository<Content, int> _repository;
        private readonly IContentProducer _contentProducer;
        public ContentController(
            IRepository<Content, int> repository,
            IContentProducer contentProducer)
        {
            _repository = repository;
            _contentProducer = contentProducer;
        }
        // GET: api/content
        [HttpGet]
        public IEnumerable<Content> Get()
        {
            var response = _repository.GetAll();
            return response;
        }

        // GET api/content/5
        [HttpGet("{id}")]
        public Content Get(int id)
        {
            var response = _repository.Get(id);
            return response;
        }

        // POST api/content
        [HttpPost]
        public async Task Post([FromBody]Content content)
        {
            await _repository.CreateAsync(content).ConfigureAwait(false);
            _contentProducer.Produce(content);
        }

        // PUT api/content/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Content content)
        {
            content.Id = id;
            _repository.Update(content);
        }

        // DELETE api/content/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
