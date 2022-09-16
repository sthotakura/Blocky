namespace BlockyWeb.Core;

public interface IPasswordManager
{
    void SetPassword(string password);

    bool IsPassword(string password);
}