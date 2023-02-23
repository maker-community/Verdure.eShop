using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Verdure.eShop.Mongo.Extensions;

/// <summary>
/// 服务注册于配置扩展
/// </summary>
public static class GridFSExtensions
{
    /// <summary>
    /// 注册GridFS服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="db">IMongoDatabase,为空情况下使用默认数据库hoyofs</param>
    /// <returns></returns>
    public static void AddHoyoGridFS(this IServiceCollection services, IMongoDatabase? db = null)
    {
        var client = services.BuildServiceProvider().GetService<IMongoClient>();
        if (db is null & client is null)
        {
           throw new("无法从容器中获取IMongoClient的服务依赖,请考虑显示传入db参数.");
        }
        var hoyo = client!.GetDatabase("hoyofs");
        _ = services.Configure<FormOptions>(c =>
        {
            c.MultipartBodyLengthLimit = long.MaxValue;
            c.ValueLengthLimit = int.MaxValue;
        }).Configure<KestrelServerOptions>(c => c.Limits.MaxRequestBodySize = int.MaxValue).AddSingleton(new GridFSBucket(hoyo));
    }
}