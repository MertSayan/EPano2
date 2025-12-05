namespace EPano2.Models.Dtos
{
    public class AnnouncementDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PosterImageUrl { get; set; }
        public string CreatedDate { get; set; }

        // Ekranı kontrol etmek için
        public int DisplayOrder { get; set; }
        
        // Duyuru/Haber ayrımı için
        public bool Haber { get; set; }
    }
}
