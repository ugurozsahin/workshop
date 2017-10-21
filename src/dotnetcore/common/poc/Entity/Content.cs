using Core.Entity;
using System;

namespace Entity
{
    public class Content : IEntity<int>
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public byte Status { get; set; }
    }
}
