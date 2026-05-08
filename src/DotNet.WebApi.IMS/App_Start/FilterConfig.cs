using System.Web;
using System.Web.Mvc;

namespace DotNet.WebApi.IMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
