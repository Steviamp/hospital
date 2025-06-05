namespace PapageorgiouHospitalQueueSystem.Helpers
{
    public interface IAesEncryptionHelper
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}

