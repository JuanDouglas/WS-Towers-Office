using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Models.Enums;

namespace WSTowersOffice.Api.Controllers
{
    public class FileModel
    {
        public int ID { get; set; }
        public int Leaght { get; set; }
        public string Filename { get; set; }
        public FileType FileType { get; set; }

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