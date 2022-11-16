using ApiServer.Model;

namespace ApiServer.WebApi.Models
{
    public class ItemModel
    {
        public Guid ItemId { get; set; }
        public string Identifier { get; set; }

        public ItemModel(Item item)
        {
            ItemId = item.ItemId;
            Identifier = item.Identifier;
        }
    }
}
