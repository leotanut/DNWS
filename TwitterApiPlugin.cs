using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DNWS
{
    class TwitterApiPlugin : TwitterPlugin
    {
        public List<User> ListUser()
        {
            using (var context = new TweetContext())
            {
                try
                {
                    List<User> users = context.Users.ToList();
                    return users;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Following> GetFollow(string name)
        {
            using (var context = new TweetContext())
            {
                try
                {
                    List<User> followings = context.Users.Where(b => b.Name.Equals(name)).Include(b => b.Following).ToList();
                    return followings[0].Following;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public override HTTPResponse GetResponse(HTTPRequest request)
        {
            HTTPResponse response = new HTTPResponse(200);

            string user = request.getRequestByKey("user");
            string password = request.getRequestByKey("password");
            string following = request.getRequestByKey("follow");
            string timeline = request.getRequestByKey("timeline");
            string message = request.getRequestByKey("message");
            string[] at = request.Filename.Split("?");
            if (at[0] == "users")
            {
                if (request.Method == "GET")
                {
                    string j = JsonConvert.SerializeObject(ListUser());
                    response.body = Encoding.UTF8.GetBytes(j);
                }
                else if (request.Method == "POST")
                {
                    if (user != null && password != null)
                    {
                        Twitter.AddUser(user, password);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    else
                    {
                        response.status = 403;
                        response.body = Encoding.UTF8.GetBytes("403 User invalid");
                    }
                }
                else if (request.Method == "DELETE")
                {
                    try
                    {
                        Twitter.DeleteUser(user);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not exists");
                    }
                }
            }
            else if (at[0] == "following")
            {
                if (request.Method == "GET")
                {

                    Twitter follow = new Twitter(user);
                    string temp = JsonConvert.SerializeObject(GetFollow(user));
                    response.body = Encoding.UTF8.GetBytes(temp);
                }
                else if (request.Method == "POST")
                {
                    if (user != null && following != null)
                    {
                        Twitter twitter = new Twitter(user);
                        twitter.AddFollowing(following);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    else
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not exists");
                    }
                }
                else if (request.Method == "DELETE")
                {
                    try
                    {
                        Twitter twitter = new Twitter(user);
                        twitter.RemoveFollowing(following);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404 User not exists");
                    }
                }
            }
          if (at[0] == "tweets")
            {
                Twitter twitter = new Twitter(user);
                if (request.Method == "GET")
                {
                    {
                        if (timeline == "users")
                        {
                            string temp = JsonConvert.SerializeObject(twitter.GetUserTimeline());
                            response.body = Encoding.UTF8.GetBytes(temp);
                        }
                        if (timeline == "follow")
                        {
                            string temp = JsonConvert.SerializeObject(twitter.GetFollowingTimeline());
                            response.body = Encoding.UTF8.GetBytes(temp);
                        }
                    }
                    if (request.Method == "POST")
                    {
                        twitter.PostTweet(message);
                        response.body = Encoding.UTF8.GetBytes("202 OK");
                    }
                }
                return response;
            }
           
            return response;
        }
    }

}