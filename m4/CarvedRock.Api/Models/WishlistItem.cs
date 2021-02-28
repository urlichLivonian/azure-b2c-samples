using System.ComponentModel.DataAnnotations;

namespace CarvedRock.Api.Models
{
    public class WishlistItem
    {        
        public long Id { get; set; }
        
        public long ItemId { get; set; }
                
        public string UserADIdentifier { get; set; }
    }
}