using _462_TheLastLike.Utils.Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;
using System.Web;

namespace _462_TheLastLike
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions();
            facebookOptions.AppId = FacebookConstants.APP_ID;
            facebookOptions.AppSecret = FacebookConstants.APP_SECRET;
            facebookOptions.Scope.Add("publish_actions");
            facebookOptions.Scope.Add("user_likes");

            facebookOptions.Provider = new FacebookAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    HttpContext.Current.Session.Add("FacebookContext", context);
                }
            };

            app.UseFacebookAuthentication(facebookOptions);
        }
    }
}