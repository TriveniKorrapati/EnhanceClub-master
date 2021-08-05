using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnhanceClub.WebUI.Helpers
{
    public class ZendeskService
    {

        public static string GetZendeskAdminToken()
        {
            string token = SiteConfigurationsWc.ZendeskEmail + "/token:" + SiteConfigurationsWc.ZendeskToken;
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            string encodeToken = System.Convert.ToBase64String(tokenBytes);
            return encodeToken;
        }

        public static string GetZendeskUserToken(string email)
        {
            string token = email + "/token:" + SiteConfigurationsWc.ZendeskToken;
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            string encodeToken = System.Convert.ToBase64String(tokenBytes);
            return encodeToken;
        }

        public static async Task<KeyValuePair<ZendeskCategoryModel, string>> GetZendeskCategoryFieldValue()
        {
            string apiResponse = string.Empty;
            ZendeskCategoryModel zendeskCategoryModel = null;

            var url = $"/api/v2/ticket_fields/{SiteConfigurationsWc.ZendeskCategoryFieldId}";

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(SiteConfigurationsWc.ZendeskDomain)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetZendeskAdminToken());

            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == (HttpStatusCode)401 || response.StatusCode == (HttpStatusCode)422)
                    {
                        apiResponse = "Could not authenticate you. Check your email address or register.";
                    }
                    else
                    {
                        apiResponse = "Error getting category field, Please contact the support team at 1-844-836-2582.";
                    }
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        zendeskCategoryModel = JsonConvert.DeserializeObject<ZendeskCategoryModel>(apiResponse);
                        apiResponse = string.Empty;
                    }
                    else
                    {
                        apiResponse = response.Content.ReadAsStringAsync().Result;
                        apiResponse = "Error getting category field, Please contact the support team at 1-844-836-2582.";
                    }
                }
            }

            return new KeyValuePair<ZendeskCategoryModel, string>(zendeskCategoryModel, apiResponse);
        }

        public static async Task<KeyValuePair<string, bool>> CreateUserZendeskTicket(ZendeskTicketModel model, string email)
        {
            string apiResponse;
            bool flag = false;

            var url = "/api/v2/requests.json";

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(SiteConfigurationsWc.ZendeskDomain)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string jsonClinicObject = JsonConvert.SerializeObject(model);

            StringContent content = new StringContent(jsonClinicObject, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    if (response.StatusCode == (HttpStatusCode)401 || response.StatusCode == (HttpStatusCode)422)
                    {
                        apiResponse = "Could not authenticate you. Check your email address or register.";
                    }
                    else
                    {
                        apiResponse = "Error submitting your query, Please contact the support team at 1-844-836-2582.";
                    }
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        flag = true;
                    }
                    else
                    {
                        apiResponse = response.Content.ReadAsStringAsync().Result;
                        apiResponse = "Error getting category field, Please contact the support team at 1-844-836-2582.";
                        flag = false;
                    }
                }
            }

            return new KeyValuePair<string, bool>(apiResponse, flag);
        }

        public static async Task<KeyValuePair<string, long>> GetZendeskUserId(string email)
        {
            string apiResponse;
            bool flag = false;
            ZendeskUserModel zendeskUserModel = null;

            var url = "/api/v2/users/me.json";

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(SiteConfigurationsWc.ZendeskDomain)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetZendeskUserToken(email));

            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == (HttpStatusCode)401 || response.StatusCode == (HttpStatusCode)422)
                    {
                        apiResponse = "Could not authenticate you. Check your email address or register.";
                    }
                    else
                    {
                        apiResponse = "Error getting category field, Please contact the support team at 1-844-836-2582.";
                    }
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        zendeskUserModel = JsonConvert.DeserializeObject<ZendeskUserModel>(apiResponse);
                        apiResponse = string.Empty;
                    }
                    else
                    {
                        apiResponse = response.Content.ReadAsStringAsync().Result;
                        apiResponse = "Error getting category field, Please contact the support team at 1-844-836-2582.";
                    }
                }
            }

            return new KeyValuePair<string, long>(apiResponse, Convert.ToInt64(zendeskUserModel.user.id));
        }

        public static async Task<KeyValuePair<string, bool>> AddUserContactOnZendesk(ZendeskUserModel model, string email)
        {
            string apiResponse;
            bool flag = false;
            long user_id = 0;

            /* Get user Id*/
            var userDetail = await GetZendeskUserId(email);

            if (userDetail.Value > 0)
            {
                user_id = userDetail.Value;

                var url = $"api/v2/users/{user_id}.json";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(SiteConfigurationsWc.ZendeskDomain)
                };

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetZendeskUserToken(email));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetZendeskAdminToken());


                var serilaizeUserJson = JsonConvert.SerializeObject(model, Formatting.None,
                  new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore
                  });


                string jsonClinicObject = JsonConvert.SerializeObject(serilaizeUserJson);

                StringContent content = new StringContent(jsonClinicObject, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await httpClient.PutAsync(url, content))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (response.StatusCode == (HttpStatusCode)401 || response.StatusCode == (HttpStatusCode)422)
                        {
                            apiResponse = "Could not authenticate you. Check your email address or register.";
                        }
                        else
                        {
                            apiResponse = "Error occurred, Please contact the support team at 1-844-836-2582.";
                        }
                    }
                    else
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                            flag = true;
                        }
                        else
                        {
                            apiResponse = response.Content.ReadAsStringAsync().Result;
                            apiResponse = "Error occurred, Please contact the support team at 1-844-836-2582.";
                            flag = false;
                        }
                    }
                }
            }
            else
            {
                apiResponse = "Error occurred, Please contact the support team at 1-844-836-2582.";
            }

            return new KeyValuePair<string, bool>(apiResponse, flag);
        }
    }
}