using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Corp.Integration.Utility.SqsPushWebSvc
{
    public partial class Heartbeat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string response = ConfigurationManager.AppSettings["HeartbeatResponse"];

            if (response.ToLower() == "exception")
            {
                throw new Exception("Config file instructed the pulse the return an exception.");
            }

            //Add other system checks as requirements are identified.

        }
    }
}