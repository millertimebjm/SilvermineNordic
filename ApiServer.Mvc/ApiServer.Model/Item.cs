
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class Item
    {
        [Key]
        public Guid ItemId { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
