using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.AwsEntities;
using EnhanceClub.Domain.Entities;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;

namespace EnhanceClub.Domain.Concrete
{
    public class CognitoAuthProvider :IAuthProvider
    {
        private DataSet ds;
        private bool _result = false;
        private IJsonSerializer _serializer = new JsonNetSerializer();
        private IDateTimeProvider _provider = new UtcDateTimeProvider();
        private IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public int StoreFrontId
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["StoreFrontId"].ToString()); }
        }

        public bool PreAuthenticate(string email, ICustomerRepository repository)
        {
            ds = repository.ValidateEmailLogin(email, StoreFrontId);

            if (ds.Tables[0].Rows.Count > 0)
            {
                _result = true;

                LoggedCustomer loggedCustomer = new LoggedCustomer
                {
                    CustomerFirstName = ds.Tables[0].Rows[0]["Customer_FirstName"].ToString(),
                    CustomerLastName = ds.Tables[0].Rows[0]["Customer_LastName"].ToString(),
                    CustomerEmail = ds.Tables[0].Rows[0]["Customer_Email"].ToString(),
                    CustomerId = Convert.ToInt32(ds.Tables[0].Rows[0]["Customer_Id"].ToString()),
                    CustomerStorefrontId = Convert.ToInt32(ds.Tables[0].Rows[0]["Storefront_id"].ToString()),
                    ProvinceStateId = Convert.ToInt32(ds.Tables[0].Rows[0]["Provincestate_Id"].ToString()),
                    CustomerIsPartial = ds.Tables[0].Rows[0]["Customer_IsPartial"] != DBNull.Value && Convert.ToBoolean(ds.Tables[0].Rows[0]["Customer_IsPartial"])
                };
                HttpContext.Current.Session["LoggedCustomer"] = loggedCustomer;

            }
            return _result;
        }

        public bool Authenticate(string email, string accessToken, ICustomerRepository repository)
        {
            // check if test/developer is logged in
            //Switch account
            DataSet dsDeveloperLogin = repository.GetDeveloperLogin(email, StoreFrontId);
            if (dsDeveloperLogin.Tables[0].Rows.Count > 0)
            {
                ds = dsDeveloperLogin;
            }
            else
            {
                ds = repository.ValidateEmailLogin(email, StoreFrontId);
            }
           

            if (ds.Tables[0].Rows.Count > 0)
            {
                _result = true;

                LoggedCustomer loggedCustomer = new LoggedCustomer
                {
                    CustomerFirstName = ds.Tables[0].Rows[0]["Customer_FirstName"].ToString(),
                    CustomerLastName = ds.Tables[0].Rows[0]["Customer_LastName"].ToString(),
                    CustomerEmail = ds.Tables[0].Rows[0]["Customer_Email"].ToString(),
                    CustomerId = Convert.ToInt32(ds.Tables[0].Rows[0]["Customer_Id"].ToString()),
                    CustomerStorefrontId = Convert.ToInt32(ds.Tables[0].Rows[0]["Storefront_id"].ToString()),
                    ProvinceStateId = Convert.ToInt32(ds.Tables[0].Rows[0]["Provincestate_Id"].ToString()),
                    CustomerIsPartial = ds.Tables[0].Rows[0]["Customer_IsPartial"] != DBNull.Value && Convert.ToBoolean(ds.Tables[0].Rows[0]["Customer_IsPartial"])
                };
                HttpContext.Current.Session["LoggedCustomer"] = loggedCustomer;

                DateTime tokenExpiryTime = GetExpiryTimestamp(accessToken);

                HttpCookie jwtToken = new HttpCookie("JWTToken", accessToken);
                jwtToken.HttpOnly = true;

                //JWTToken.Expires.Add(new TimeSpan(0, 1, 0));

                jwtToken.Expires = tokenExpiryTime;
                HttpContext.Current.Response.Cookies.Add(jwtToken);

              
            }
            return _result;
        }

        public DateTime GetExpiryTimestamp(string accessToken)
        {
            try
            {
                IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);

                var token = decoder.DecodeToObject<JwtToken>(accessToken);

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(token.Exp);
                return dateTimeOffset.LocalDateTime;
            }

            catch (TokenExpiredException)
            {
                return DateTime.MinValue;
            }

            catch (SignatureVerificationException)
            {
                return DateTime.MinValue;
            }
          
           
            catch (Exception ex)
            {
                // ... remember to handle the generic exception ...
                return DateTime.MinValue;
            }
        }
    }
}
