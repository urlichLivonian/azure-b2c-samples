using System;
using System.Net;
using System.Reflection;

namespace CarvedRock.Api
{
    public class B2CResponseContent
    {
        public string version { get; set; }
        public int status { get; set; }
        public string userMessage { get; set; }

        public B2CResponseContent(string message, HttpStatusCode status)
        {
            userMessage = message;
            this.status = (int)status;
            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
