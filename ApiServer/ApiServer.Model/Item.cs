
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class Item
    {
        [Key]
        public Guid ItemId { get; set; }
        public string Identifier { get; set; }
        public string? Value { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<ItemKey> ItemKeys { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj as Item != null)
            {
                return Equals((Item)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(Item item)
        {
            return item.ItemId == ItemId;
        }
    }
}
