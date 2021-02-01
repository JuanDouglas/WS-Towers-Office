using System;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Http;
using WSTowersOffice.Api.Models.Enums;
using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Properties;

namespace WSTowersOffice.Api.Controllers
{
    [RoutePrefix("api/Files")]
    public partial class FilesController : ApiController
    {
        public WSTowersOfficeEntities db => new WSTowersOfficeEntities();
        [HttpGet]
        [Route("Image")]
        // GET: api/Files
        public async Task<IHttpActionResult> Get(string filename, FileType filetype, ImageExtension extension)
        {
            //Models.Image imgModel = await db.Image.FirstOrDefaultAsync(fs => fs.FileName == filename);
            //if (imgModel == null)
            //{
            //    return NotFound();
            //}

            //string basePath = GetPartialDirectory(filetype);
            //string path = $"{basePath}{filename}";

            //if (!File.Exists(path))
            //{
            //    return NotFound();
            //}

            //FileStream flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //System.Drawing.Image img = System.Drawing.Image.FromStream(flStream);

            Bitmap bitmap = null;
            if (filename== "default_team_icon.png"&&filetype==FileType.TeamIcon)
            {
                bitmap = Resources.default_team_icon;
            }
            if (bitmap == null)
            {
                bitmap = (Bitmap)Bitmap.FromFile(GetPartialDirectory(filetype) + filename);
            }
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, GetFormat(extension));

            HttpResponseMessage message = new HttpResponseMessage
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue($"image/{extension}");
            return ResponseMessage(message);
        }
        internal static async Task<FileModel> SaveImageAsync(FileType fileType, HttpPostedFile file)
        {
            WSTowersOfficeEntities db = new WSTowersOfficeEntities();
            string fileName = $"{fileType}_{DateTime.Now.ToString("yyMMdd_HHmmss_fffffff")}.png";
            System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream);
            SaveInAPI(fileType, image, fileName);
            try
            {
                db.File.Add(new Models.File()
                {
                    FileName = fileName,
                    FileType = (int)fileType,
                    Width = image.Width,
                    Height = image.Height,
                    Leaght = file.ContentLength
                });
                await db.SaveChangesAsync();
                return new FileModel(await db.File.FirstOrDefaultAsync(fs => fs.FileName == fileName));
            }
            catch (Exception e)
            {
                throw e;
            }

            throw new NotImplementedException();
        }
        internal static async Task<FileModel> SaveImageAsync(FileType fileType, Image image, int contentLength)
        {
            WSTowersOfficeEntities db = new WSTowersOfficeEntities();
            string fileName = $"{fileType}_{DateTime.Now.ToString("yyMMdd_HHmmss_fffffff")}.png";
            SaveInAPI(fileType, image, fileName);
            try
            {
                db.File.Add(new Models.File()
                {
                    FileName = fileName,
                    FileType = (int)fileType,
                    Width = image.Width,
                    Height = image.Height,
                    Leaght = contentLength
                });
                await db.SaveChangesAsync();
                return new FileModel(await db.File.FirstOrDefaultAsync(fs => fs.FileName == fileName));
            }
            catch (Exception e)
            {
                throw e;
            }

            throw new NotImplementedException();
        }
        private static void SaveInAPI(FileType fileType, System.Drawing.Image image, string filename)
        {
            var path = GetPartialDirectory(fileType);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream fs = new FileStream($"{path}{filename}", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                image.Save(fs, ImageFormat.Png);
            }
        }
        private static string GetPartialDirectory(FileType type)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            switch (type)
            {
                case FileType.TeamIcon:
                    return baseDirectory + "\\Files\\Images\\Team\\Icon";
                default:
                    return baseDirectory;
            }
        }
        public ImageFormat GetFormat(ImageExtension extension)
        {
            switch (extension)
            {
                case ImageExtension.jpeg:
                    return ImageFormat.Jpeg;
                case ImageExtension.bmp:
                    return ImageFormat.Bmp;
                case ImageExtension.png:
                    return ImageFormat.Png;
                case ImageExtension.icon:
                    return ImageFormat.Icon;
                case ImageExtension.gif:
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}