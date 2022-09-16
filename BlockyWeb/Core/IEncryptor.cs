namespace BlockyWeb.Core;

public interface IEncryptor
{
    string Encrypt(string password);
}