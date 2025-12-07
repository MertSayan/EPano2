namespace EPano2.Models
{
    /// <summary>
    /// Kayan yazı için özel metin öğesi. Admin birden fazla ticker yazısı ekleyebilir ve istediğini aktif edebilir.
    /// </summary>
    public class TickerItem
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Kayan yazı metni
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Bu ticker yazısı aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Görüntülenme sırası
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

