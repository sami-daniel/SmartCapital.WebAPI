using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;

namespace SmartCapital.WebAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EntityFrameworkCoreTransactionControllerFilter : ActionFilterAttribute
    {
        private IDbContextTransaction _transaction = null!;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var db = (DbContext)context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext))!;
            _transaction = db.Database.BeginTransaction();

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (_transaction != null)
            {
                if (context.Exception != null)
                {
                    _transaction.RollbackAsync();
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
