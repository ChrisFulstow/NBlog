using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Application
{
    public class AdminOnlyAttribute : FilterAttribute, IAuthorizationFilter
    {
        public IServices Services { get; set; }

        private readonly bool _authorize;

        public AdminOnlyAttribute()
        {
            _authorize = true;
        }

        public AdminOnlyAttribute(bool authorize)
        {
            _authorize = authorize;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (AuthorizeCore(filterContext.HttpContext))
            {
                var cache = filterContext.HttpContext.Response.Cache;
                cache.SetProxyMaxAge(new TimeSpan(0L));
                cache.AddValidationCallback(CacheValidateHandler, null);
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!_authorize) { return true; }

            return Services.User.Current.IsAdmin;
        }

        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!AuthorizeCore(httpContext))
                return HttpValidationStatus.IgnoreThisRequest;

            return HttpValidationStatus.Valid;
        }

        protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
    }
}