using Circular.Core.Entity;
using Circular.Framework.Middleware.Emailer;

namespace Circular
{
    public interface ICommon
    {
         Customers CurrentUser();
        string UserId { get; set; }

    }
}
