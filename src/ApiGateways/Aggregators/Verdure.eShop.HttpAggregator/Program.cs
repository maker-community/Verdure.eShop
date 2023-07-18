using Microsoft.AspNetCore.Rewrite;
using Verdure.eShop.HttpAggregator.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Host.AddYarpJson();// 添加Yarp的配置文件

// 添加Yarp反向代理ReverseProxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    //app.UseSwaggerUIWithYarp();
    //app.UseRewriter(new RewriteOptions()
    //    // Regex for "", "/" and "" (whitespace)
    //    .AddRedirect("^(|\\|\\s+)$", "/swagger"));
}

app.MapGet("/Ping", () => "Hello World!");
app.MapReverseProxy();
app.Run();