using System;
using System.Linq;
using ipm_quickstart_csharp_mac.Models;
using Microsoft.AspNet.Mvc;
using Twilio;
using Twilio.IpMessaging;
using ipm_quickstart_csharp_mac.Extensions;

namespace ipm_quickstart_csharp_mac.Controllers
{
    public class HomeController : Controller
    {
        private FriendsContext _db;
        public HomeController()
        {
            _db = new FriendsContext();
        }

        
        public IActionResult Index()
        {
            return View(_db.Friends);
        }
        
        public IActionResult MessageAdded(string To, string From, string Body){
            var client = new TwilioRestClient(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"));
            
            using (var _db = new FriendsContext())
            {
                var channelSid = _db.Friends.FirstOrDefault(s => s.ChannelSid == To).PhoneNumber;
                
                Message message = client.SendMessage(
                    StringExtensions.MyNumber,
                    channelSid,
                    Body
                );
                
                if (message.RestException!=null)
                {
                    //an exception occurred making the REST call
                    string result = message.RestException.Message;
                    return Content(result);
                }
            }
            return Content(string.Empty);
        }

        public IActionResult SMSAdded(string To, string From, string Body)
        {
            var client = new IpMessagingClient(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"));
            using (var _db = new FriendsContext()){
                var channelSid = _db.Friends.FirstOrDefault(s => s.PhoneNumber == From).ChannelSid;
            
                var message = client.CreateMessage(
                    Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"),
                    channelSid,
                    From.RemoveSpecialCharacters(),
                    Body
                );
                    
                if (message.RestException!=null) {
                    string result = message.RestException.Message;
                    return Content(result);
                }
            }
            
            return Content(string.Empty);
        }
    }
}
