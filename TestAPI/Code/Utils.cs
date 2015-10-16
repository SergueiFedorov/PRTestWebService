using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestAPI.Models;

namespace TestAPI.Code
{
    public static class Utils
    {
        public static bool CheckToken(string token)
        {
            using (DatabaseModel model = new DatabaseModel())
            {
                return model.SecurityTokens.Any(x => x.Token == token) == false;
            }
        }
    }
}