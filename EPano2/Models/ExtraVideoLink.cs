namespace EPano2.Models
{
    /// <summary>
    /// Varsayılan video dışında sırayla döndürülecek ekstra video linkleri.
    /// </summary>
    public class ExtraVideoLink
    {
        public int Id { get; set; }

        public int VideoConfigId { get; set; }
        public VideoConfig VideoConfig { get; set; } = null!;

        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Videoların hangi sırayla döneceğini belirlemek için.
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}


