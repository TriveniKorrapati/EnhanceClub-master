using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EnhanceClub.Domain.AwsHelper;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class AdminDbLayer
    {

        // readonly string _sCon = ConfigurationManager.ConnectionStrings["Connection"].ToString();
        private readonly string _sCon = @SiteConfigurations.SCon;

        DataSet _ds;

        // Add Entry to Action Log
        public int AddActionLog(string logController, string logAction, string logIp, DateTime logDateTime, bool logSessionTimeout)
        {
                             
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogAction", true);

                paramCollection[1].Value = logController;
                paramCollection[2].Value = logAction;
                paramCollection[3].Value = logIp;
                paramCollection[4].Value = logDateTime;
                paramCollection[5].Value = logSessionTimeout;
                paramCollection[6].Value = null; // new Action Log Id
                paramCollection[7].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogAction", paramCollection);

                var msg = paramCollection[7].Value;
              
                var value = paramCollection[6].Value;
                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       // log unexpected happenings during code execution
        public int AddUnexpectedLog(string message, string module, string relevantInfo, int customerId, int storefrontId, int productsizeId, int productId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogUnexpected", true);

                paramCollection[1].Value = message;
                paramCollection[2].Value = module;
                paramCollection[3].Value = relevantInfo.Replace("'","''");

                paramCollection[4].Value = customerId;
                paramCollection[5].Value = storefrontId;
                paramCollection[6].Value = productsizeId;
                paramCollection[7].Value = productId;
                
                paramCollection[8].Value = null;  // new Log Id
                paramCollection[9].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogUnexpected", paramCollection);

                var msg = paramCollection[9].Value;

                var value = paramCollection[8].Value;
                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // check if ip is canadian ip
        public bool IsCanadianIp(long ipNum)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsCanadianIP", true);

                paramCollection[1].Value = ipNum;
                paramCollection[2].Value = null;  // is canadian

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsCanadianIP", paramCollection);
                
                var value = paramCollection[2].Value;
                if (value != null) return Convert.ToBoolean(value.ToString());

                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add log email exception
        public int AddLogEmailException(string toEmail, string mailSubject, string mailFrom, string errorMessage,
            string serverIp, string serverPort, int storefrontFk, int customerFk, int orderInvoiceFk, DateTime createdDate)
        {

            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogEmailException", true);

                paramCollection[1].Value = toEmail;
                paramCollection[2].Value = mailSubject;
                paramCollection[3].Value = mailFrom;
                paramCollection[4].Value = errorMessage;
                paramCollection[5].Value = serverIp;
                paramCollection[6].Value = serverPort;
                paramCollection[7].Value = storefrontFk;
                paramCollection[8].Value = customerFk;
                paramCollection[9].Value = orderInvoiceFk;
                paramCollection[10].Value = createdDate;
                paramCollection[11].Value = null; //Log Exception id
                paramCollection[12].Value = null; //message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogEmailException", paramCollection);

                var value = paramCollection[11].Value;
                if (value != null)
                    return Convert.ToInt32(value.ToString());
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int AddLogSendEmailReminder(int userAdminFk, int storefrontFk, int customerFk, int orderInvoiceFk,
                                      string customerEmail, string customerPhone, DateTime dateCreated,
                                      int emailType, string emailSubject, string emailContent, int applicationFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_be_AddLogSendEmailReminder", true);
                paramCollection[1].Value = null; // UpdateStatus
                paramCollection[2].Value = null; // Message

                paramCollection[3].Value = userAdminFk;
                paramCollection[4].Value = storefrontFk;
                paramCollection[5].Value = customerFk;
                paramCollection[6].Value = orderInvoiceFk;
                paramCollection[7].Value = customerEmail;
                paramCollection[8].Value = customerPhone;
                paramCollection[9].Value = dateCreated;
                paramCollection[10].Value = emailType;
                paramCollection[11].Value = emailSubject;
                paramCollection[12].Value = emailContent.Replace("'", "''");
                paramCollection[13].Value = applicationFk;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_be_AddLogSendEmailReminder", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
   
}

