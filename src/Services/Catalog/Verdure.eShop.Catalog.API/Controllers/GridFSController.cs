using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Verdure.eShop.Services.Catalog.API.Controllers;
[Route("api/[controller]"), ApiController]
public class GridFSController : ControllerBase
{
    /// <summary>
    /// GridFSBucket
    /// </summary>
    private readonly GridFSBucket Bucket;

    public GridFSController(GridFSBucket bucket)
    {
        Bucket = bucket;
    }

    [HttpPost]
    [RequestSizeLimit(1048576)]
    [HttpPost("UploadPicture")]
    public async Task<ActionResult<UploadImageDto>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file selected.");

        if (file.Length > 1048576)
            return BadRequest("File size exceeds 1 MB.");

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Pics", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host}/api/Pics/{fileName}";

        var dto = new UploadImageDto
        {
            ImageUrl = imageUrl,
            FileName = fileName
        };

        return Ok(dto);
    }


    private readonly FilterDefinitionBuilder<GridFSFileInfo> gbf = Builders<GridFSFileInfo>.Filter;

    /// <summary>
    /// 添加一个文件
    /// </summary>
    [HttpPost("UploadSingle")]
    public async Task<string> PostSingle([FromForm] IFormFile fs)
    {
        if (fs is null) throw new("no files find");
        if (fs.ContentType is null) throw new("ContentType in File is null");
        var metadata = new Dictionary<string, object> { { "contentType", fs.ContentType } };
        var upo = new GridFSUploadOptions
        {
            BatchSize = 1,
            Metadata = new(metadata)
        };
        var oid = await Bucket.UploadFromStreamAsync(fs.FileName, fs.OpenReadStream(), upo);
        return oid.ToString();
    }

    /// <summary>
    /// 添加一个或多个文件
    /// </summary>
    [HttpPost("UploadMulti")]
    public async Task<IEnumerable<string>> PostMulti([FromForm] IFormFileCollection fs)
    {
        if (fs is null || fs.Count == 0) throw new("no files find");
        var result = new List<string>();
        foreach (var item in fs)
        {
            if (item.ContentType is null) throw new("ContentType in File is null");
            var metadata = new Dictionary<string, object> { { "contentType", item.ContentType } };
            var upo = new GridFSUploadOptions
            {
                BatchSize = fs.Count,
                Metadata = new(metadata)
            };
            var oid = await Bucket.UploadFromStreamAsync(item.FileName, item.OpenReadStream(), upo);
            result.Add(oid.ToString());
        }
        return result;
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns></returns>
    [HttpGet("Download/{id}")]
    public async Task<FileStreamResult> Download(string id)
    {
        var stream = await Bucket.OpenDownloadStreamAsync(ObjectId.Parse(id), new() { Seekable = true });
        return File(stream, stream.FileInfo.Metadata["contentType"].AsString, stream.FileInfo.Filename);
    }

    /// <summary>
    /// 通过文件名下载
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("DownloadByName/{name}")]
    public async Task<FileStreamResult> DownloadByName(string name)
    {
        var stream = await Bucket.OpenDownloadStreamByNameAsync(name, new() { Seekable = true });
        return File(stream, stream.FileInfo.Metadata["contentType"].AsString, stream.FileInfo.Filename);
    }

    /// <summary>
    /// 打开文件内容
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns></returns>
    [HttpGet("FileContent/{id}")]
    public async Task<FileContentResult> FileContent(string id)
    {
        var fi = await (await Bucket.FindAsync(gbf.Eq(c => c.Id, ObjectId.Parse(id)))).SingleOrDefaultAsync() ?? throw new("no data find");
        var bytes = await Bucket.DownloadAsBytesAsync(ObjectId.Parse(id), new GridFSDownloadOptions() { Seekable = true });
        return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
    }

    /// <summary>
    /// 通过文件名打开文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("FileContentByName/{name}")]
    public async Task<FileContentResult> FileContentByName(string name)
    {
        var fi = await (await Bucket.FindAsync(gbf.Eq(c => c.Filename, name))).FirstOrDefaultAsync() ?? throw new("can't find this file");
        var bytes = await Bucket.DownloadAsBytesByNameAsync(name, new() { Seekable = true });
        return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
    }
    /// <summary>
    /// 重命名文件
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <param name="newName">新名称</param>
    /// <returns></returns>
    [HttpPut("{id}/Rename/{newName}")]
    public async Task Rename(string id, string newName)
    {
        await Bucket.RenameAsync(ObjectId.Parse(id), newName);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="ids">文件ID集合</param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IEnumerable<string>> Delete(params string[] ids)
    {
        var oids = ids.Select(ObjectId.Parse).ToList();
        var fi = await (await Bucket.FindAsync(gbf.In(c => c.Id, oids))).ToListAsync();
        var fids = fi.Select(c => new
        {
            Id = c.Id.ToString(),
            FileName = c.Filename
        }).ToArray();
        Task DeleteSingleFile()
        {
            foreach (var item in fids)
            {
                _ = Bucket.DeleteAsync(ObjectId.Parse(item.Id));
            }
            return Task.CompletedTask;
        }
        _ = fids.Length > 6 ? Task.Run(DeleteSingleFile) : DeleteSingleFile();
        return fids.Select(c => c.FileName);
    }
}
