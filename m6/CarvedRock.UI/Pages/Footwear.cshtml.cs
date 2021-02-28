using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using CarvedRock.UI.Services;
using CarvedRock.UI.Models;

namespace CarvedRock.UI.Pages
{
    public class FootwearModel : PageModel
    {
        IWishlistService _wishlistService;

        public List<Footwear> FootwearProducts { get; set; }

        public FootwearModel(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;

            FootwearProducts = new List<Footwear>();
        }

        public async Task OnGetAsync()
        {
            if (FootwearProducts.Count == 0)
            {
                for (int i = 1; i < 4; i++)
                {
                    FootwearProducts.Add(
                        new Footwear { ItemId = i, ItemName = $"Boot {i}", OnWishlist = false }
                    );
                }
            }
            
            if (User.Identity.IsAuthenticated)
            {
                // Get the wishlist items
                foreach (var item in await _wishlistService.GetAllItems())
                {
                    FootwearProducts.Single(f => f.ItemId == item.ItemId).OnWishlist = true;
                }
            }                        
        }

        public async Task OnPostWishlistAsync(int itemId)
        {
            System.Diagnostics.Debug.WriteLine($"post with item id: {itemId}");

            await _wishlistService.AddItem(itemId);
        }
    }
}
