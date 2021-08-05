using EnhanceClub.WebUI.AwsHelper;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Owin;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace EnhanceClub.WebUI
{
    public partial class Startup
    {

        private static readonly string PoolId = AwsConfiguration.PoolId;

        private static readonly string ClientAppId = AwsConfiguration.ClientId;


        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            ConfigureAWSCognitoAuthTokenConsumption(app);
        }

        private void ConfigureAWSCognitoAuthTokenConsumption(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (HttpContext.Current.Request.Cookies["JWTToken"] != null)
                {
                    string jwtToken = HttpContext.Current.Request.Cookies["JWTToken"].Value;

                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        //Dictionary<string, string> abc = new Dictionary<string, string>();
                        //abc["JWToken"] = "Bearer " + jwtToken;

                        context.Request.Headers.Set("Authorization", "Bearer " + jwtToken);
                    }
                }
                await next();
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "JWTToken",
                LoginPath = new PathString("/Customer/Login"),
                CookieHttpOnly = true
            });

            var cognitoIssuer = $"https://cognito-idp.ca-central-1.amazonaws.com/" + PoolId;
            var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
            var cognitoAudience = ClientAppId; // app client id

            app.UseJwtBearerAuthentication(
                new Microsoft.Owin.Security.Jwt.JwtBearerAuthenticationOptions
                {

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                        {
                            // get JsonWebKeySet from AWS
                            var json = new WebClient().DownloadString(jwtKeySetUrl);

                            // serialize the result
                            var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

                            // cast the result to be the type expected by IssuerSigningKeyResolver
                            return (IEnumerable<SecurityKey>)keys;
                        },
                        ValidIssuer = cognitoIssuer,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = false
                        //ValidAudience = cognitoAudience

                    }

                });

        }

        //// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        //public void ConfigureAuth(IAppBuilder app)
        //{
        //    // Enable the application to use a cookie to store information for the signed in user
        //    app.UseCookieAuthentication(new CookieAuthenticationOptions
        //    {
        //        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        //        LoginPath = new PathString("/Account/Login")
        //    });
        //    // Use a cookie to temporarily store information about a user logging in with a third party login provider
        //    app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        //    // Uncomment the following lines to enable logging in with third party login providers
        //    //app.UseMicrosoftAccountAuthentication(
        //    //    clientId: "",
        //    //    clientSecret: "");

        //    //app.UseTwitterAuthentication(
        //    //   consumerKey: "",
        //    //   consumerSecret: "");

        //    //app.UseFacebookAuthentication(
        //    //   appId: "",
        //    //   appSecret: "");

        //    //app.UseGoogleAuthentication();
        //}
    }
}