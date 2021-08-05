using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EnhanceClub.WebUI.Helpers;
using EnhanceClub.WebUI.Models;

namespace EnhanceClub.WebUI.HtmlHelpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href",pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("active");
                    tag.AddCssClass("");
                }
                tag.AddCssClass("");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

        // this generates anchor tags embedded in li for formatting to use current css styling
        public static MvcHtmlString PageLinksLi(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            
            StringBuilder result = new StringBuilder();
           
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder li = new TagBuilder("li");
                if (i == pagingInfo.CurrentPage)
                {
                    li.AddCssClass("active");
                }
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
               
                // add new generated anchor tag to li
                li.InnerHtml = tag.ToString();

                result.Append(li);
            }
            return MvcHtmlString.Create(result.ToString());
        }



        // this generates anchor tags embeded in li for formatting to use current css styling
        public static MvcHtmlString PageLinksGroup(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {

            StringBuilder result = new StringBuilder();

            result.Append("<li><a>Page</a></li>");

            // find start and end of pagelinks group
            CommonFunctions.NumberRange numRange = new CommonFunctions.NumberRange();
            numRange.GroupSize = pagingInfo.LinkCount;
            numRange.NumberToFindGroup = pagingInfo.CurrentPage;
            numRange.MaxGroup = pagingInfo.TotalPages;

            numRange.FindGroup();
            int startPage = numRange.GroupStart;
            int uptoPage = numRange.GroupEnd;

            if (pagingInfo.CurrentPage > pagingInfo.LinkCount)
            {
                // first page link
                TagBuilder liFirst = new TagBuilder("li");

                TagBuilder tagFirst = new TagBuilder("a");
                tagFirst.MergeAttribute("href", pageUrl(1));
                tagFirst.InnerHtml = "<i class=\"fa fa-angle-double-left\" aria-hidden=\"true\"></i>"; ;

                // add new generated anchor tag to li
                liFirst.InnerHtml = tagFirst.ToString();

                result.Append(liFirst);

                // previous page link

                TagBuilder lip = new TagBuilder("li");

                TagBuilder tagp = new TagBuilder("a");
                tagp.MergeAttribute("href", pageUrl(startPage - pagingInfo.LinkCount));
                tagp.InnerHtml = "<i class=\"fa fa-angle-left\" aria-hidden=\"true\"></i>";

                // add new generated anchor tag to li
                lip.InnerHtml = tagp.ToString();

                result.Append(lip);



            }
            for (int i = startPage; i <= uptoPage; i++)
            {
                TagBuilder li = new TagBuilder("li");
                if (i == pagingInfo.CurrentPage)
                {
                    li.AddCssClass("active");
                }
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();

                // add new generated anchor tag to li
                li.InnerHtml = tag.ToString();

                result.Append(li);
            }

            // add next page link
            if (pagingInfo.CurrentPage < pagingInfo.TotalPages - pagingInfo.LinkCount)
            {
                TagBuilder liNext = new TagBuilder("li");

                TagBuilder tagNext = new TagBuilder("a");
                tagNext.MergeAttribute("href", pageUrl(startPage + pagingInfo.LinkCount));
                tagNext.InnerHtml = "<i class=\"fa fa-angle-right\" aria-hidden=\"true\"></i>";

                // add new generated anchor tag to li
                liNext.InnerHtml = tagNext.ToString();

                result.Append(liNext);

                // Last Page link
                TagBuilder liLast = new TagBuilder("li");

                TagBuilder tagLast = new TagBuilder("a");
                tagLast.MergeAttribute("href", pageUrl(pagingInfo.TotalPages));
                tagLast.InnerHtml = "<i class=\"fa fa-angle-double-right\" aria-hidden=\"true\"></i>"; ;

                // add new generated anchor tag to li
                liLast.InnerHtml = tagLast.ToString();

                result.Append(liLast);
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public static string MyValidationSummary(this HtmlHelper html, string validationMessage)
        {
            if (!html.ViewData.ModelState.IsValid)
            {
                return "<div class=\"validation-summary\">" + html.ValidationSummary(validationMessage) + "</div>";
            }

            return "";
        }
    }
}
    

   
