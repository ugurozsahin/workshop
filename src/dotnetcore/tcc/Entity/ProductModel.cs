using Core.Entity;
using System;

namespace Entity
{
    [Serializable]
    public class ProductModel : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }
}
