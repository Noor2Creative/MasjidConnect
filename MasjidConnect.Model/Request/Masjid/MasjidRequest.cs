using Microsoft.AspNetCore.Http;

namespace MasjidConnect.Model.Request.Masjid
{    
    public class MasjidRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? OtherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? ContactPerson { get; set; } = string.Empty;
        public string? ContactNo { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int EnteredBy { get; set; } = 0;
        public List<IFormFile>? MasjidImage { get; set; }
        public string? ImagesXml { get; set; } = string.Empty;
        //public PrayerTime? PrayerTime { get; set; }
    }
    public class NamazTimeRequest
    {
        public int Id { get; set; }
        public int MasjidId { get; set; }
        public string Fajr { get; set; } = string.Empty;
        public string Dhuhr { get; set; } = string.Empty;
        public string Asr { get; set; } = string.Empty;
        public string Maghrib { get; set; } = string.Empty;
        public string Isha { get; set; } = string.Empty;
        public string JumaKhutba { get; set; } = string.Empty;
        public int EnteredBy { get; set; } = 0;
    }
}
