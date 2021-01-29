using System;
using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Models.Enums;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace WSTowersOffice.Api.Models
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
        public string ImageUrl { get => $"{UriBuilder.Scheme}://{UriBuilder.Host}/api/Files/filename={Filename}&fileType={FileType}"; }
        public FileModel(File file)
        {
            Leaght = file.Leaght;
            Filename = file.FileName;
            FileType = (FileType)file.FileType;
            ID = file.ID;
        }
        public FileModel()
        {
        }
    }
}