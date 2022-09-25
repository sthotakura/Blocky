namespace BlockyApi.Core;

public interface IEncryptor
{
    string Encrypt(string password);

    string Decrypt(string encrypted);
}