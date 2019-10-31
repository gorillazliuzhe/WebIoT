using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebIoT.Filter
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger; 
        public GlobalExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GlobalExceptionFilter>();
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await Task.Run(() =>
            {
                var json = new ErrorResponse("未知错误,请重试")
                {
                    Name = context.ActionDescriptor.GetType().GetProperty("ActionName")?.GetValue(context.ActionDescriptor).ToString(),
                    Path = $"链接访问出错：{context.HttpContext.Request.Path}",
                    Msg = context.Exception.Message,
                    // Data = context.Exception // 会使System.Text.Jso序列化失败
                };
                if (context.Exception is OperationCanceledException)
                {
                    json.Msg = "一个请求在浏览器被取消";
                    context.ExceptionHandled = true;
                    context.Result = new StatusCodeResult(400);
                }
                else
                {
                    _logger.LogError(new EventId(context.Exception.HResult), context.Exception, System.Text.Json.JsonSerializer.Serialize(json));
                    context.Result = new RedirectResult("/Home/Error");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.ExceptionHandled = true; //设置异常已经处理,否则会被其他异常过滤器覆盖
                }
            });
        }
    }
    public class ErrorResponse
    {
        public ErrorResponse(string msg)
        {
            Msg = msg;
        }

        public int Status { get; set; } = 0;

        public string Name { get; set; }
        public string Path { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}
