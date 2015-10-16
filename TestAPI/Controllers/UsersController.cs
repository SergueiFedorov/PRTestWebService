using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using TestAPI.Code;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    public class UserRespose
    {
        //This is really silly, but because of the Entity Framework the Json parsing will break on User
        public dynamic User { get; set; }
        public string Error { get; set; }
    }

    public class UserRequest
    {
        public string Token { get; set; }
        public User User { get; set; }
    }


    public class UsersController : ApiController
    {
        //Utils
        private UserRespose _Package(string error, User user)
        {
            return new UserRespose
            {
                //See comment in UserResponse
                User = new
                {
                    Username = user?.Username,
                    name = user?.Name,
                    UserId = user?.UserId,
                },
                Error = error
            };
        }

        private UserRespose _TokenFailed()
        {
            return _Package("The token provided is invalid", null);
        }

        private UserRespose _UserRequestFailed()
        {
            return _Package("Request is null or User data was not provided", null);
        }

        private bool _CheckUserRequest(UserRequest request)
        {
            return request == null || request.User == null;
        }

        private UserRespose _RunAllChecks(string token, UserRequest request)
        {
            if (Utils.CheckToken(token) )
            {
                return _TokenFailed();
            }

            if (_CheckUserRequest(request))
            {
                return _UserRequestFailed();
            }

            return null;
        }


        // GET: api/Users/5
        public UserRespose Get([FromUri] string token, [FromUri] int userId)
        {
            if (Utils.CheckToken(token))
            {
                return _TokenFailed();
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                return _Package(null, model.Users.SingleOrDefault(x => x.UserId == userId));
            }
        }

        // POST: api/Users
        public UserRespose Post([FromBody]UserRequest request)
        {
            var checkResult = _RunAllChecks(request.Token, request);
            if (checkResult != null)
            {
                return checkResult;
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                model.Users.Add(request.User);
                model.SaveChanges();

                return  _Package(null, request.User);
            }
        }

        // PUT: api/Users/5
        public UserRespose Put([FromBody]UserRequest request)
        {
            var checkResult = _RunAllChecks(request.Token, request);
            if (checkResult != null)
            {
                return checkResult;
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                var modelToUpdate = model.Users.SingleOrDefault(x => x.UserId == request.User.UserId);

                modelToUpdate.Name = request.User.Name;
                modelToUpdate.Username = request.User.Username;

                model.SaveChanges();

                return _Package(null, request.User);
            }
        }

        // DELETE: api/Users/5
        public UserRespose Delete([FromBody]UserRequest request)
        {
            var checkResult = _RunAllChecks(request.Token, request);
            if (checkResult != null)
            {
                return checkResult;
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                Expression<Func<User, bool>> predicate = x => x.UserId == request.User.UserId;

                if (model.Users.Any(predicate) == false)
                {
                    return _Package("UserId was not found", null);
                }

                if (model.Journeys.Any(x => x.UserId == request.User.UserId))
                {
                    return _Package("A journey is currently attatched this user. You cannot delete a user with existing journeys", null);
                }

                model.Users.Remove(model.Users.SingleOrDefault(predicate));
                model.SaveChanges();

                return _Package(null, null);
            }
        }
    }
}
