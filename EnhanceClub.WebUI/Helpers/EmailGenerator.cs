using System;
using Castle.Core.Internal;
using EnhanceClub.WebUI.Models;
using RazorEngine.Templating;


namespace EnhanceClub.WebUI.Helpers
{
    public class EmailGenerator
    {
        // use template to generate order shipped email

        public static string GenerateNewsLetterConfirm(NewsLetterSignUpViewModel emailModel)
        {
            var emailHtmlBody = "";
            var templateService = new TemplateService();

            if (emailModel.EmailTemplatePath != null)
            {
                String templateFilePath = "";

                templateFilePath = System.Web.HttpContext.Current.Server.MapPath(emailModel.EmailTemplatePath + "NewsLetter.cshtml");
                
                if (!templateFilePath.IsNullOrEmpty())
                {

                    emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath), emailModel, null, null);
                }

            }
            return emailHtmlBody;
        }
    }
}