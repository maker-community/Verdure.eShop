using MongoDB.Driver;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Verdure.eShop.Mongo;

/// <summary>
/// 创建一个基础的DBContext来管理一些MongoDB的设置和创建基本的对象.
/// </summary>
public class BaseDbContext 
{
    /// <summary>
    /// MongoClient
    /// </summary>
    public IMongoClient Client { get; private set; } = default!;
    /// <summary>
    /// 获取链接字符串或者HoyoMongoSettings中配置的特定名称数据库或默认数据库hoyo
    /// </summary>
    public IMongoDatabase Database { get; private set; } = default!;

    /// <summary>
    ///  使用链接字符串创建客户端,并提供字符串中的数据库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connStr">链接字符串</param>
    /// <param name="db">数据库名称</param>
    /// <returns></returns>
    internal static T CreateInstance<T>(string connStr, string db = "hoyo") where T : BaseDbContext
    {
        var t = Activator.CreateInstance<T>();
        var mongoUrl = new MongoUrl(connStr);
        var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
        t.Client = new MongoClient(clientSettings);
        var dbName = !string.IsNullOrWhiteSpace(mongoUrl.DatabaseName) ? mongoUrl.DatabaseName : db;
        t.Database = t.Client.GetDatabase(dbName);
        return t;
    }
}