namespace BlockyApi.Core;

public sealed class PasswordManager : IPasswordManager, IDisposable
{
    const string PasswordFile = "Block.password";

    readonly IEncryptor _encryptor;
    readonly Stream _passwordFileStream;

    public PasswordManager(IEncryptor encryptor)
    {
        _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));

        EncryptedPassword = File.Exists(PasswordFile) ? File.ReadAllText(PasswordFile) : string.Empty;
        _passwordFileStream = File.Open(PasswordFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    string EncryptedPassword { get; set; }

    public void SetPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
        
        if (!string.IsNullOrWhiteSpace(EncryptedPassword))
        {
            throw new InvalidOperationException("password can only be set once.");
        }

        EncryptedPassword = _encryptor.Encrypt(password);
        var writer = new StreamWriter(_passwordFileStream);
        writer.WriteLine(EncryptedPassword);
    }

    public bool IsPassword(string password) => EncryptedPassword == _encryptor.Encrypt(password);

    public bool HasPassword() => !string.IsNullOrWhiteSpace(EncryptedPassword);

    public void Dispose()
    {
        _passwordFileStream.Dispose();
    }
}