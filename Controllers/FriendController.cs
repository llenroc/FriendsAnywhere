using System;
using System.Linq;
using ipm_quickstart_csharp_mac.Models;
using Microsoft.AspNet.Mvc;
using Twilio.IpMessaging;
using ipm_quickstart_csharp_mac.Extensions;

namespace ipm_quickstart_csharp_mac.Controllers
{
    public class FriendController : Controller
    {
        private FriendsContext _db;
        private IpMessagingClient _client;

        public FriendController()
        {
            _db = new FriendsContext();
            _client = new IpMessagingClient(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"), Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"));
        }

        public IActionResult Index()
        {
            return View(_db.Friends);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        //POST: /Friend/Create/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Friend friend)
        {
            ModelState.Clear();
            TryValidateModel(friend);
            if (ModelState.IsValid)
            {
                using (var db = new FriendsContext())
                {
                    // Create a channel                    
                    var channel = _client.CreateChannel(Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"), "public", friend.Number, friend.Number, string.Empty);
                    
                    // join this channel
                    if (channel.RestException!=null)
                    {
                        //an exception occurred making the REST call
                        return Content(channel.RestException.Message);
                    }
                    else{
                        // Create a user
                        var user = _client.CreateUser(Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"), StringExtensions.RemoveSpecialCharacters(friend.Number));
                        if (user.RestException!=null){
                            return Content(user.RestException.Message);
                        }
                        else{
                            // Create membership
                            var member = _client.CreateMember(Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"), channel.Sid, user.Identity, string.Empty);
                            if (member.RestException!=null){
                                return Content(member.RestException.Message);
                            }
                            else{
                                // Add complete user to the DB
                                friend.ChannelSid = channel.Sid;
                                friend.UserSid = user.Sid;
                                db.Friends.Add(friend);
                                db.SaveChanges();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(friend);
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var friend = _db.Friends.FirstOrDefault(s => s.FriendId == id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            return View(friend);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Friend friend)
        {
            ModelState.Clear();
            TryValidateModel(friend);
            if (ModelState.IsValid)
            {
                using (var db = new FriendsContext())
                {
                    db.Friends.Update(friend);
                    var count = db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(friend);
        }
        
        public IActionResult Delete(int id)
        {
            var friend = _db.Friends.FirstOrDefault(s => s.FriendId == id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Delete channel                
                _client.DeleteChannel(Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"), friend.ChannelSid);
                _client.DeleteUser(Environment.GetEnvironmentVariable("TWILIO_IPM_SERVICE_SID"), friend.UserSid);
                
                // Remove user from from the database
                _db.Friends.Remove(friend);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
