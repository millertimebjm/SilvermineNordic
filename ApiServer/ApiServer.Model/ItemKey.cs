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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj as ItemKey != null)
            {
                return Equals((ItemKey)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(ItemKey itemKey)
        {
            return itemKey.ItemKeyId == ItemKeyId;
        }
    }
}
