using System.Security.Cryptography;

namespace TourPlanner.Bll.Auth;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // random value for each password
    private const int KeySize = 32; // created hash is 32 bytes long
    private const int Iterations = 100_000; // PBKDF2 is 100.000 times used

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256; // hash algorithm within PBKDF2

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize); // generate a random salt for each password
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize); // hash including variables

        return string.Join( // format like PBKDF2.100000.salt.hash (salt needed for verification)
            '.',
            "PBKDF2",
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.'); // split hash into components
        if (parts.Length != 4 || parts[0] != "PBKDF2" || !int.TryParse(parts[1], out var iterations))
        { // correct format?
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]); // get salt from hash
        var expectedHash = Convert.FromBase64String(parts[3]); // get hash from hash
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedHash.Length); // hash the password with the same variables

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash); // compare result with stored hash using a fixed time comparison to prevent timing attacks
    }
}
