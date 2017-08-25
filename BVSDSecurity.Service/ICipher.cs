namespace BvsdSecurity.Service {
    public interface ICipher {
        byte[] IV { get; set; }
        bool NonStandard { get; set; }

        byte[] Crypt_CTR(byte[] text, int numThreads);
        byte[] Decrypt_CBC(byte[] ct);
        string Decrypt_CBC(string ct);
        string Decrypt_CTR(string ct);
        byte[] Decrypt_ECB(byte[] ct);
        string Decrypt_ECB(string ct);
        byte[] Encrypt_CBC(byte[] pt);
        string Encrypt_CBC(string pt);
        string Encrypt_CTR(string pt);
        byte[] Encrypt_ECB(byte[] pt);
        string Encrypt_ECB(string pt);
        byte[] SetRandomIV();
    }
}