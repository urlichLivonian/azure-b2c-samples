using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarvedRock.Api.Models;

namespace CarvedRock.Api.Data
{
    public static class SeedData 
    {
        public static void Initialize(CarvedRockContext context)
        {
            context.WishlistItems.AddRange(
                new WishlistItem {                    
                    UserADIdentifier = "asdf",
                    ItemId = 1
                },
                new WishlistItem
                {                    
                    UserADIdentifier = "asdf",
                    ItemId = 2
                }
            );

            context.SaveChanges();
        }
    }
}