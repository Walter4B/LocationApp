using Microsoft.AspNetCore.Mvc.Filters;

namespace LocationApp.Account.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new BasicAuthFilter();
        }

        public bool IsReusable => false;
    }
}
