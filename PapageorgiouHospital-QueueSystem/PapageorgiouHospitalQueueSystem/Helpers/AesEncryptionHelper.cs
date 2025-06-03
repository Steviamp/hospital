using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace PapageorgiouHospitalQueueSystem.Helpers
{
    public class AesEncryptionHelper : IAesEncryptionHelper
    {
        private readonly EncryptionSettings _settings;

        public AesEncryptionHelper(IOptions<EncryptionSettings> options)
        {
            _settings = options.Value;
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_settings.Key);
            aes.IV = Encoding.UTF8.GetBytes(_settings.IV);

            var encryptor = aes.CreateEncryptor();
            var input = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string encryptedText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_settings.Key);
            aes.IV = Encoding.UTF8.GetBytes(_settings.IV);

            var decryptor = aes.CreateDecryptor();
            var buffer = Convert.FromBase64String(encryptedText);
            var decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
