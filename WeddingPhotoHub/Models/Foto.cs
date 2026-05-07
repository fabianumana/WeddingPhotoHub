namespace WeddingPhotoHub.Models
{
    public class Foto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string Url { get; set; }
        public DateTime FechaSubida { get; set; }
        public long Tamaño { get; set; } 
    }
}
