using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopping.WebUI.HtmlHelpers
{
    public static class SidebarNavHelpers
    {
        public static MvcHtmlString SideBarLinks(this HtmlHelper html,
            Func<int, string> pageUrl)
        {

            StringBuilder result = new StringBuilder();
            TagBuilder tag = new TagBuilder("li");
            TagBuilder subTag = new TagBuilder("a"); // Construct an <a> tag 
            subTag.MergeAttribute("href", pageUrl(1));
            // subTag.InnerHtml = "<span class='badge pull-right'>"+42+"</span>";
            tag.InnerHtml = subTag.ToString();
//                    if (i == pagingInfo.CurrentPage)
//                        tag.AddCssClass("active ");
            result.Append(tag.ToString());
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString HighlightSelected(this HtmlHelper html,string categoryName,string selectedCategory)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder tag = new TagBuilder("li");

            if(categoryName == selectedCategory)
                tag.MergeAttribute("class","active");
            result.Append(tag.ToString());

            return MvcHtmlString.Create(result.ToString());
        }
 
    }
}