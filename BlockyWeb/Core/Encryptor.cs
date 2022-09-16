using Microsoft.AspNetCore.DataProtection;

namespace BlockyWeb.Core;

public sealed class Encryptor : IEncryptor
{
    readonly IDataProtector _protector;

    public Encryptor(IDataProtector protector)
    {
        _protector = protector;
    }
    
    public string Encrypt(string password) => _protector.Protect(password);
}