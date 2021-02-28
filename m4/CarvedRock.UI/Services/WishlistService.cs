using System;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

using CarvedRock.UI.Models;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using System.Collections.Generic;

namespace CarvedRock.UI.Services
{
    public static class WishlistServiceExtensions
    {
        public static void AddWishlistService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IWishlistService, WishlistService>();
        }
    }

    public class WishlistService : IWishlistService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _readScope = string.Empty;
        private readonly string _writeScope = string.Empty;
        private readonly string _baseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public WishlistService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _readScope = configuration["Wishlist:readscope"];
            _writeScope = configuration["Wishlist:writescope"];
            _baseAddress = configuration["Wishlist:baseAddress"];
        }

        public async Task<List<WishlistItem>> GetAllItems()
        {
            var createReq = await CreateAuthenticatedRequest(HttpMethod.Get, $"{_baseAddress}/wishlist");

            if (createReq == null)
                return new List<WishlistItem>();

            var resp = await _httpClient.SendAsync(createReq);

            var rawItems = await resp.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<List<WishlistItem>>(rawItems);

            return items;
        }

        public async Task AddItem(int itemId)
        {            
            var item = new WishlistItem { ItemId = itemId, UserADIdentifier = "asdf" };

            var itemJson = JsonConvert.SerializeObject(item);

            var createReq = await CreateAuthenticatedRequest(
                HttpMethod.Post, $"{_baseAddress}/wishlist"
            );

            if (createReq == null)
                return;

            createReq.Content = new StringContent(itemJson);
            createReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var resp = await _httpClient.SendAsync(createReq);
            
            System.Diagnostics.Debug.WriteLine(resp.StatusCode);
        }

        public async Task DeleteItem(int itemId)
        {
            await Task.CompletedTask;
        }

        private async Task<HttpRequestMessage> CreateAuthenticatedRequest(HttpMethod method, string requestUrl)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(method, requestUrl);

                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                    new[] { _readScope, _writeScope }
                );

                System.Diagnostics.Debug.WriteLine($"access token: {accessToken}");
                                
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer", accessToken
                );

                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                return null;
            }
        }        
    }
}