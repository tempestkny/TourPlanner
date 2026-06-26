using Microsoft.EntityFrameworkCore;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class UserRepositoryTests
{
    string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
    IUserRepository UserRepository;
    TourPlannerDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
        .UseNpgsql(connectionString)
        .Options;
        context = new TourPlannerDbContext(options);

        UserRepository = new UserRepository(context);

        
    }

    [TearDown]
    public async Task TearDown()
    {
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"users\" RESTART IDENTITY CASCADE");
        context?.Dispose();
    }

    [Test]
    public async Task CreateUser_ShouldFindInDataBase()
    {
        var user = new User()
        {
            Email = "myuser@gmail.com",
            Username = "MyUserName",
            HashedPassword = "MyPassword"
        };

        await UserRepository.Create(user);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.users.FirstOrDefault(u => u.Email == user.Email), Is.Not.Null);
            Assert.That(context.users.FirstOrDefault(u => u.Username == user.Username), Is.Not.Null);
            Assert.That(context.users.FirstOrDefault(u => u.HashedPassword == user.HashedPassword), Is.Not.Null);

        }

    }

    [Test]
    public async Task FindUserInDataBase()
    {
        var user = new User
        {
            Email = "maxmustermann@outlook.com",
            Username = "Max Mustermann",
            HashedPassword = "MusterPasswort"
        };

        context.users.Add(user);
        context.SaveChanges();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await UserRepository.Read(user.Id), Is.Not.Null);
            Assert.That(await UserRepository.GetUserByName(user.Username), Is.Not.Null);
        }

    }

    [Test]
    public async Task UpdateUserEmailAndName()
    {
        var user = new User
        {
            Email = "janedoe@hotmail.com",
            Username = "Jane Doe",
            HashedPassword = "password123"
        };

        context.users.Add(user);
        await context.SaveChangesAsync();

        await UserRepository.Update(user.Id, new User
        {
           Email = "johndoe@hotmail.com",
           Username = "John Doe"
        });

        Assert.That(user.Email, Is.EqualTo("johndoe@hotmail.com"));
        Assert.That(user.Username, Is.EqualTo("John Doe"));
    }

    [Test]
    public async Task UpdateUserPassword()
    {
        var user = new User
        {
            Email = "janedoe@hotmail.com",
            Username = "Jane Doe",
            HashedPassword = "password123"
        };
        context.users.Add(user);
        await context.SaveChangesAsync();

        await UserRepository.UpdatePassword(user.Id,"B0w71ng.B477");

        Assert.That(user.HashedPassword, Is.EqualTo("B0w71ng.B477"));
    }
    [Test]
    public async Task DeleteUser_ShouldReturnNull()
    {
        var user = new User
        {
            Email = "maxmustermann@outlook.com",
            Username = "Max Mustermann",
            HashedPassword = "MusterPasswort"
        };

        context.users.Add(user);
        context.SaveChanges();

        await UserRepository.Delete(user);

        Assert.That(context.users.Find(user.Id),Is.Null);
    }
}

