using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WIQuest.Web.API.Responses
{
    /// <summary>
    /// Response zur Abfrage der UserId
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Die Guid UserId als String
        /// </summary>
        public string uid { get; set; }
        public string name { get; set; }
        public string points { get; set; }
        public string rounds { get; set; }
        public string field_of_study { get; set; }
        public string semester { get; set; }
    }


    public class RegisterResponse
    {
        public bool error { get; set; }

        public string error_msg { get; set; }
    }
}