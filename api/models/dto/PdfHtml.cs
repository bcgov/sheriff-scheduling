namespace CAS.API.models.dto
{
    public class PdfHtml
    {
        public string html { get; set; }
        public string recipients { get; set; }
        public string emailContent { get; set; }
        public string emailSubject { get; set; }
    }
}