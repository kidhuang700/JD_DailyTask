﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace JD_DailyTask
{
    /// <summary>
    /// 自定义异常过滤器
    /// </summary>
    public class CustomerExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger;

        public CustomerExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 重写OnExceptionAsync方法，定义自己的处理逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.Error(context.Exception, $"自定义异常过滤器：{context.Exception.Message}");

            // 如果异常没有被处理则进行处理
            if (context.ExceptionHandled == false)
            {
                // 定义返回类型
                var result = new Models.ApiResultModel
                {
                    Code = -1,
                    Message = context.Exception.Message
                };
                context.Result = new ContentResult
                {
                    // 返回状态码设置为200，表示成功
                    StatusCode = StatusCodes.Status200OK,
                    // 设置返回格式
                    ContentType = "application/json;charset=utf-8",
                    Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                    {
                        // 设置为驼峰命名
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
            }
            // 设置为true，表示异常已经被处理了
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
