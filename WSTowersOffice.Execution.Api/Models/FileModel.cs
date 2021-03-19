using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace WSTowersOffice.Execution.Api.Models
{
    public class FileModel
    {
        public int ID { get; set; }
        public int Leaght { get; set; }
        public string Filename { get; set; }
        public FileType FileType { get; set; }
        private UriBuilder UriBuilder => new UriBuilder(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
        [JsonIgnore]
        [XmlIgnore]
        public string ImageUrl
        {
            get
            {
                UriBuilder builder = UriBuilder;
                builder.Path = "api/Files/Image";
                builder.Query = $"filename={Filename}&filetype={FileType}&extension={ImageExtension.png}";
                return builder.ToString();
            }
        }
        public FileModel(File file)
        {
            if (file != null)
            {
                Leaght = file.Leaght;
                Filename = file.FileName;
                FileType = (FileType)file.FileType;
                ID = file.ID;
            }
        }
        public FileModel()
        {
        }
    }
}