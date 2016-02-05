using System;
using Microsoft.AspNet.Mvc;
using Twilio.Auth;
using ipm_quickstart_csharp_mac.Extensions;

namespace TwilioIpMessaging.Controllers
{
    public class TokenController : Controller
    {
        // GET: /token
        public IActionResult Index(string Device)
        {
            // Load Twilio configuration from Web.config
            var AccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var ApiKey = Environment.GetEnvironmentVariable("TWILIO_IPM_KEY");
            var ApiSecret = Environment.GetEnvironmentVariable("TWILIO_IPM_SECRET");
            var IpmServiceSid = Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID");

            // Create a random identity for the client
            var Identity = StringExtensions.RemoveSpecialCharacters(StringExtensions.MyName);

            // Create an Access Token generator
            var Token = new AccessToken(AccountSid, ApiKey, ApiSecret);
            Token.Identity = Identity;

            // Create an IP messaging grant for this token
            var grant = new IpMessagingGrant();
            grant.EndpointId = $"QuickStartService:{Identity}:{Device}";            
            grant.ServiceSid = IpmServiceSid;
            Token.AddGrant(grant);

            return Json(new {
                identity = Identity,
                token = Token.ToJWT()
            });
        }
    }
}