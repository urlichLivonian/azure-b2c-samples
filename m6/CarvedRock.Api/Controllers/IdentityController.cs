using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CarvedRock.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarvedRock.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly CarvedRockContext _context;

        public IdentityController(CarvedRockContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(InputClaimsModel inputClaims)
        {
            try
            {
                if (inputClaims == null)
                {
                    var c = new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Content = JsonConvert.SerializeObject(new B2CResponseContent("Cannot deserialize input claims", HttpStatusCode.Conflict))
                    };

                    return c;
                }
                
                int loyaltyNumber;

                // Check to see if user already in loyalty program
                if (_context.LoyaltyProgramInfo.Count(l => l.ADObjectId == inputClaims.oid) > 0)
                {
                    loyaltyNumber = _context.LoyaltyProgramInfo.First(l =>
                        l.ADObjectId == inputClaims.oid).LoyaltyNumber;
                }
                else
                {
                    loyaltyNumber = new Random().Next(100, 1000);

                    var loyaltyProgramInfo = new LoyaltyProgram{
                        ADObjectId = inputClaims.oid,
                        LoyaltyNumber = loyaltyNumber
                    };

                    _context.LoyaltyProgramInfo.Add(loyaltyProgramInfo);
                    await _context.SaveChangesAsync();
                }

                // Create an output claims object and set the loyalty number with a random value
                OutputClaimsModel outputClaims = new OutputClaimsModel();
                outputClaims.loyaltyNumber = loyaltyNumber.ToString();

                // Return the output claim(s)
                return new OkObjectResult(outputClaims);
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Content = ex.ToString()
                };
            }
        }
    }
}