using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WebIoT.Filter
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception == null) return;
            var json = new ErrorResponse
            {
                Name = context.ActionDescriptor.GetType().GetProperty("ActionName")?.GetValue(context.ActionDescriptor)?.ToString(),
                Path = context.HttpContext.Request.Path,
                Msg = context.Exception.ToString()
            };
            if (!(context.Exception is TaskCanceledException) && !(context.Exception is OperationCanceledException))
            {
                var errormsg = JsonConvert.SerializeObject(json);
                _logger.LogError(new EventId(context.Exception.HResult), context.Exception, errormsg);
                if (!_env.IsDevelopment())
                {
                    context.Result = new RedirectResult("/Home/Error");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.ExceptionHandled = true;   //设置异常已经处理,否则会被其他异常过滤器覆盖,这样页面将不会展示异常[黄页面]
                }
            }
            await Task.CompletedTask;
        }
    }

    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Msg { get; set; }
    }
}
