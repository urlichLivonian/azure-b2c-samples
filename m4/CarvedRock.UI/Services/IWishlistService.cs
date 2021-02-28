using System.Collections.Generic;
using System.Threading.Tasks;
using CarvedRock.UI.Models;

namespace CarvedRock.UI.Services
{
    public interface IWishlistService
    {
         Task<List<WishlistItem>> GetAllItems();
         Task AddItem(int itemId);
         Task DeleteItem(int itemId);
    }
}