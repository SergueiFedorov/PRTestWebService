using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestAPI.Code;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    public class JourneyRespose
    {
        //This is really silly, but because of the Entity Framework the Json parsing will break on User
        public dynamic Journey { get; set; }
        public string Error { get; set; }
    }

    public class JourneyRequest
    {
        public string Token { get; set; }
        public Journey Journey { get; set; }
    }

    public class JourneysController : ApiController
    {
        private JourneyRespose _Package(string error, Journey journey)
        {
            return new JourneyRespose
            {
                //See comment in UserResponse
                Journey = new
                {
                    Name = journey?.Name,
                    CreatedDate = journey?.CreatedDate,
                    UserId = journey?.UserId,
                    JourneyId = journey?.JourneyId
                },
                Error = error
            };
        }

        private bool _CheckJourneyRequest(JourneyRequest request)
        {
            return request == null || request.Journey == null;
        }

        private JourneyRespose _TokenFailed()
        {
            return _Package("The token provided is invalid", null);
        }

        private JourneyRespose _UserRequestFailed()
        {
            return _Package("Request is null or Journey data was not provided", null);
        }


        private JourneyRespose _RunAllChecks(string token, JourneyRequest request)
        {
            if (Utils.CheckToken(token))
            {
                return _TokenFailed();
            }

            if (_CheckJourneyRequest(request))
            {
                return _UserRequestFailed();
            }

            return null;
        }

        // GET: api/Journeys/5
        public JourneyRespose Get([FromUri] string token, [FromUri] int journeyId)
        {
            if (Utils.CheckToken(token))
            {
                return _TokenFailed();
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                return _Package(null, model.Journeys.SingleOrDefault(x => x.UserId == journeyId));
            }
        }

        // POST: api/Journeys
        public JourneyRespose Post([FromBody]JourneyRequest request)
        {
            var errorCheck = _RunAllChecks(request.Token, request);
            if (errorCheck != null)
            {
                return errorCheck;
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                if (model.Users.Any(x => x.UserId == request.Journey.UserId) == false)
                {
                    return _Package("A journey must be assigned to a user", null);
                }

                model.Journeys.Add(request.Journey);
                model.SaveChanges();

                return _Package(null, request.Journey);
            }
         }

        // PUT: api/Journeys/5
        public JourneyRespose Put([FromBody]JourneyRequest request)
        {
            if (Utils.CheckToken(request.Token))
            {
                return _TokenFailed();
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                var modelToUpdate = model.Journeys.SingleOrDefault(x => x.JourneyId == request.Journey.JourneyId);
                modelToUpdate.Name = request.Journey.Name;
                model.SaveChanges();

                return _Package(null, modelToUpdate);
            }
        }

        // DELETE: api/Journeys/5
        public JourneyRespose Delete([FromBody]JourneyRequest request)
        {
            if (Utils.CheckToken(request.Token))
            {
                return _TokenFailed();
            }

            using (DatabaseModel model = new DatabaseModel())
            {
                Expression<Func<Journey, bool>> predicate = x => x.JourneyId == request.Journey.JourneyId;

                if (model.Journeys.Any(predicate) == false)
                {
                    return _Package("JourneyId was not found", null);
                }

                model.Journeys.Remove(model.Journeys.Single(predicate));
                model.SaveChanges();

                return _Package(null, null);
            }
        }
    }
}
