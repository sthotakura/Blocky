using Microsoft.AspNetCore.DataProtection;

namespace BlockyApi.Core;

public sealed class Encryptor : IEncryptor
{
    readonly IDataProtector _protector;

    public Encryptor(IDataProtectionProvider protectionProvider)
    {
        _protector =
            (protectionProvider ?? throw new ArgumentNullException(nameof(protectionProvider)))
            .CreateProtector("Blocky");
    }

    public string Encrypt(string password) => _protector.Protect(password);
}