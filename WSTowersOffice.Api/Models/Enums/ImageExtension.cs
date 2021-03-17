using Newtonsoft.Json;

namespace WSTowersOffice.Api.Models.Enums
{
    public enum ImageExtension : uint
    {
        [JsonProperty("JPEG")]
        jpeg = 0,
        [JsonProperty("BMP")]
        bmp = 1,
        [JsonProperty("PNG")]
        png = 2,
        [JsonProperty("ICON")]
        icon = 3,
        [JsonProperty("GIF")]
        gif = 4
    }
}