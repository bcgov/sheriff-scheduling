using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using CAS.API.infrastructure.exceptions;

namespace CAS.API.infrastructure.encryption
{
    public class AesGcmXmlEncryptor : IXmlEncryptor
    {
        private readonly byte[] _key;

        public AesGcmXmlEncryptor(IServiceProvider services)
        {
            var options = services.GetRequiredService<AesGcmEncryptionOptions>();
            _key = Encoding.UTF8.GetBytes(options.Key);
            if (_key.Length != 32)
                throw new ConfigurationException("Key length not 32 bytes (256 bits), it is currently " + _key.Length);
        }

        public EncryptedXmlInfo Encrypt(XElement plaintextElement)
        {
            if (plaintextElement == null)
                throw new ArgumentNullException(nameof(plaintextElement));

            using var aesObj = new AesGcmService(_key);

            var element = new XElement("encryptedKey",
                new XComment(" This key is encrypted with AES-256-GCM. "),
                new XElement("value",
                    aesObj.Encrypt(plaintextElement.ToString())));

            return new EncryptedXmlInfo(element, typeof(AesGcmXmlDecryptor));
        }
    }
}
