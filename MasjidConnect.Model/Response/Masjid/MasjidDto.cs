namespace MasjidConnect.Model.Response.Masjid
{
    public class MasjidRespose
    {
        public ResponseHeader ResponseHeader { get; set; }
        public List<MasjidDto>? MasjidDto { get; set; }        
    }
    public class MasjidDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public PrayerTimeDto? PrayerTimeDto { get; set; }
    }

    public class PrayerTimeDto
    {
        public int Id { get; set; }
        public int MasjidId { get; set; }
        public string Date { get; set; }
        public string Fajr { get; set; } = string.Empty;
        public string Dhuhr { get; set; } = string.Empty;
        public string Asr { get; set; } = string.Empty;
        public string Maghrib { get; set; } = string.Empty;
        public string Isha { get; set; } = string.Empty;
        public string JumaKhutba { get; set; } = string.Empty;
    }
}
