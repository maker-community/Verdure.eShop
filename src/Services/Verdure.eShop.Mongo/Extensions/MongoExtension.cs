using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Verdure.eShop.Mongo.Extensions;

public static class MongoExtension
{
    /// <summary>
    /// 通过连接字符串添加DbContext
    /// </summary>
    /// <typeparam name="T">Hoyo.Mongo.DbContext</typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="connStr">链接字符串</param>
    /// <returns></returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddMongoDbContext<T>(this IServiceCollection services, string connStr) where T : BaseDbContext
    {
        RegistryConventionPack();
        var db = BaseDbContext.CreateInstance<T>(connStr);
        _ = services.AddSingleton(db).AddSingleton(db.Database).AddSingleton(db.Client);
        return services;
    }
    /// <summary>
    /// 注册Pack
    /// </summary>
    private static void RegistryConventionPack(bool first = true)
    {
        if (first)
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),// 驼峰命名字段
                new IgnoreExtraElementsConvention(true),
                new NamedIdMemberConvention("Id","ID"),// 将_id字段映射为Id或者ID
                new EnumRepresentationConvention(BsonType.String),// 将枚举类型存储为字符串值
            };
            ConventionRegistry.Register($"hoyo-pack-{Guid.NewGuid()}", pack, _ => true);
            BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Local));// 将时间转化为本地时间
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(new DateOnlySerializer());
        }
        var id_pack = new ConventionPack
        {
            new StringObjectIdIdGeneratorConvention()//Id[string] mapping ObjectId
        };
        ConventionRegistry.Register($"id-pack{Guid.NewGuid()}", id_pack, _ => true);
    }
}

/// <summary>
/// map the [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId]
/// </summary>
internal class StringObjectIdIdGeneratorConvention : ConventionBase, IPostProcessingConvention
{
    public void PostProcess(BsonClassMap classMap)
    {
        var idMemberMap = classMap.IdMemberMap;
        if (idMemberMap is null || idMemberMap.IdGenerator is not null) return;
        if (idMemberMap.MemberType == typeof(string)) _ = idMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(BsonType.ObjectId));
    }
}