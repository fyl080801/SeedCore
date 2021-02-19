using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ModuleScript(this IHtmlHelper helper)
        {
            return new HtmlString("<script></script>");
        }
    }
}