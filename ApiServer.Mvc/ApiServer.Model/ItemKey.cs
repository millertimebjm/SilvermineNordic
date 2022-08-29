using System;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class ItemKey
    {
        [Key]
        public Guid ItemKeyId { get; set; }
        public Guid Key { get; set; }
        public string Note { get; set; }
        public Item Item { get; set; }
        public Guid ItemId { get; set; }
    }
}
