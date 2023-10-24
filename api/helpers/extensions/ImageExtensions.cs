using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAS.API.helpers.extensions
{
    public static class ImageExtensions
    {

        public static bool IsImage(this byte[] fileBytes)
        {
            var headers = new List<byte[]>
            {
                Encoding.ASCII.GetBytes("GIF"),     // GIF
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 255, 216, 255, 224 },  // JPEG
                new byte[] { 255, 216, 255, 225 }   // JPEG CANON
            };

            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }
    }
}
