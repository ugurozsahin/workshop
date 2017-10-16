using System.Collections.Generic;
using Core.Data;
using Core.Entity;
using Entity;
using Nest;
using System;
using System.Threading.Tasks;
using System.Linq;
using Core.Config;

namespace Data.ElasticSearch
{
    public class ContentRepository : IRepository<Content, int>
    {
        private const string INDEX_NAME = "content";
        private readonly ElasticClient _elasticClient;

        private readonly IApplicationConfig _applicationConfig;

        public ContentRepository(IApplicationConfig applicationConfig)
        {
            _applicationConfig = applicationConfig;
            _elasticClient = InitializeElasticSearchClient(_applicationConfig.ConnectionStrings.ElasticSearch);
        }

        private ElasticClient InitializeElasticSearchClient(string uris)
        {
            var uriList = uris.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => new Uri(s));
            var sniffingConnectionPool = new Elasticsearch.Net.SniffingConnectionPool(uriList);
            var connSettings = new ConnectionSettings(sniffingConnectionPool);
            connSettings.BasicAuthentication("elastic", "changeme");
            connSettings.ConnectionLimit(-1);
#if Debug
            connSettings.DisableDirectStreaming();
#endif
            connSettings.EnableHttpCompression();
            connSettings.DefaultIndex(INDEX_NAME);
            var elasticClient = new ElasticClient(connSettings);

            var indexExistsReponse = elasticClient.IndexExists(INDEX_NAME);
            if (!indexExistsReponse.Exists)
            {
                var createIndexResponse = elasticClient.CreateIndex(INDEX_NAME, desc => desc.Mappings(m => m.Map<Content>(ms => ms.AutoMap())));
                if (!createIndexResponse.Acknowledged)
                    throw new ApplicationException($"ElasticSearch Create Index Throw Exception : {createIndexResponse.ServerError}", createIndexResponse.OriginalException);
            }

            return elasticClient;
        }

        public async Task CreateAsync(IEntity<int> item)
        {
            var createResponse = await _elasticClient.CreateAsync(item, desc => desc
                .Id(item.Id)
                .Refresh(Elasticsearch.Net.Refresh.True));
        }

        public void Delete(int id)
        {
            var deleteResponse = _elasticClient.Delete(DocumentPath<Content>.Id(id));
        }

        public Content Get(int id)
        {
            var response = _elasticClient.Get(DocumentPath<Content>.Id(id));
            return response.Source;
        }

        public IList<Content> GetAll()
        {
            var response = _elasticClient.Search<Content>(desc => desc
            .From(0)
            .Size(1000));
            return response.Documents.ToList();
        }

        public void Update(IEntity<int> item)
        {
            var updateResponse = _elasticClient.Update(DocumentPath<Content>.Id(item.Id), desc => desc
            .Doc((Content)item)
            .DocAsUpsert());
        }
    }
}
