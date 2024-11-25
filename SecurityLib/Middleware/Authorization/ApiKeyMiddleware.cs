using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLib.Middleware.Authorization
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
    }
}
