using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarvedRock.Api.Data;
using CarvedRock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;


namespace CarvedRock.Api.Controllers
{
    //TODO: Change the scopes values to your tenant's propertiess
    [ApiController]
    [Route("[controller]")]    
    public class WishlistController : ControllerBase
    {
        private readonly CarvedRockContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistController(CarvedRockContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;            
        }

        [HttpGet]
        [AuthorizeForScopes(Scopes = new string[] {
            "https://carvedrock.onmicrosoft.com/api/wishlist.read"
        })]
        public ActionResult<List<WishlistItem>> GetAll()
        {
            var adObjectId = _httpContextAccessor.HttpContext.User.GetObjectId();

            return _context.WishlistItems.Where(wi => wi.UserADIdentifier == adObjectId).ToList();
        }

        [HttpGet("{id}")]
        [AuthorizeForScopes(Scopes = new string[] {
            "https://carvedrock.onmicrosoft.com/api/wishlist.read"
        })]
        public async Task<ActionResult<WishlistItem>> GetById(long id)
        {
            var adObjectId = _httpContextAccessor.HttpContext.User.GetObjectId();

            // Not sure if this will ever get used
            var wishlistItem = await _context.WishlistItems
                .Where(wi => wi.UserADIdentifier == adObjectId && wi.Id == id)
                .SingleOrDefaultAsync();                

            if (wishlistItem == null)
                return NotFound();

            return wishlistItem;
        }

        [HttpPost]
        [AuthorizeForScopes(Scopes = new string[] {            
            "https://carvedrock.onmicrosoft.com/api/wishlist.write"
        })]        
        public async Task<ActionResult<WishlistItem>> Create(WishlistItem item)
        {
            var adObjectId = _httpContextAccessor.HttpContext.User.GetObjectId();
            item.UserADIdentifier = adObjectId;

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id}, item);
        }

        [HttpDelete("{id}")]
        [AuthorizeForScopes(Scopes = new string[] {
            "https://carvedrock.onmicrosoft.com/api/wishlist.write"
        })]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.WishlistItems.FindAsync(id);

            if (item == null)
                return NotFound();

            var adObjectId = _httpContextAccessor.HttpContext.User.GetObjectId();
            if (item.UserADIdentifier != adObjectId)
                return BadRequest();

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}