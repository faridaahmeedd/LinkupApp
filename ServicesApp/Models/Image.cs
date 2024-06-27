namespace ServicesApp.Models
{
    public class Image
    {
        public int Id { get; set; }
        public required byte[] Img { get; set; }
        public required ServiceRequest ServiceRequest { get; set; }


    }
}
