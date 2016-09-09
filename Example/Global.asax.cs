﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Sharp;
using Sharp.EndPoints;

namespace Example
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        { 
            var app = new AssemblyParser();

            app.AddAssembly(typeof(Sharp.EndPoints.Sitemap).Assembly);
            app.AddAssembly(typeof(Global).Assembly);
            
            app.AddGlobalParsers(new RouteParser());

            app.Parse();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //replace TemplatePlugin.current file with your own 
            Context.Response.StatusCode = 500;
            new Sharp.EndPoints.Error(sender, e);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}