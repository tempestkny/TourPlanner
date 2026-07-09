using TourPlanner.Bll.Auth;

namespace TourPlanner.Tests;

[TestFixture]
public class PasswordHasherTests
{
    private Pbkdf2PasswordHasher passwordHasher;

    [SetUp]
    public void Setup()
    {
        passwordHasher = new Pbkdf2PasswordHasher();
    }

    [Test]
    public void HashPassword_ShouldReturnHashDifferentFromPlaintextPassword()
    { // correct format
        const string password = "CorrectPassword123";

        var hash = passwordHasher.HashPassword(password);

        Assert.That(hash, Is.Not.EqualTo(password));
        Assert.That(hash, Does.StartWith("PBKDF2."));
    }

    [Test]
    public void HashPassword_ShouldUseDifferentSaltForSamePassword()
    { // different hashes
        const string password = "CorrectPassword123";

        var firstHash = passwordHasher.HashPassword(password);
        var secondHash = passwordHasher.HashPassword(password);

        Assert.That(firstHash, Is.Not.EqualTo(secondHash));
    }

    [Test]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    { // password matches the hash
        const string password = "CorrectPassword123";
        var hash = passwordHasher.HashPassword(password);

        var result = passwordHasher.VerifyPassword(password, hash);

        Assert.That(result, Is.True);
    }

    [Test]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    { // password does not match the hash
        var hash = passwordHasher.HashPassword("CorrectPassword123");

        var result = passwordHasher.VerifyPassword("WrongPassword123", hash);

        Assert.That(result, Is.False);
    }

    [Test]
    public void VerifyPassword_WithInvalidHashFormat_ShouldReturnFalse()
    { // hash is not in the expected format
        var result = passwordHasher.VerifyPassword("Password123", "invalid-hash");

        Assert.That(result, Is.False);
    }
}
