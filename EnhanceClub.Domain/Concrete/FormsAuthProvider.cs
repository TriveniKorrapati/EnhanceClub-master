using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Concrete
{
    public class FormsAuthProvider :IAuthProvider
    {
        private DataSet ds;
        private bool _result = false;
        
        public int StoreFrontId
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["StoreFrontId"].ToString()); }
        }


        public bool Authenticate(string email, string password, ICustomerRepository repository)
         {

             ds = repository.ValidateLogin(email, password, StoreFrontId);           
             
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

                 FormsAuthentication.SetAuthCookie(loggedCustomer.CustomerFirstName, false);
             }
             return _result;
         }

        public bool PreAuthenticate(string email, ICustomerRepository repository)
        {
            return true;
        }
    }
}
