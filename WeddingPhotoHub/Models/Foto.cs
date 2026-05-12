namespace WeddingPhotoHub.Models
{
    public class Foto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public string PublicId { get; set; } = string.Empty;

        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
        public long Tamaño { get; set; }
        public string? ContentType { get; set; }
    }
}
