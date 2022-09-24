namespace BlockyApi.Core;

public interface IPasswordManager
{
    void SetPassword(string password);

    bool IsPassword(string password);

    bool HasPassword();
}