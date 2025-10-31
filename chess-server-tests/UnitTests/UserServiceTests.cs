using chess_server_tests.Mocks;
using chess_server.Models;
using chess_server.Services;
using Shared.Exceptions;
using Shared.InputDtos;

namespace chess_server_tests.UnitTests;

[TestClass]
public sealed class UserServiceTests
{
    [TestMethod]
    public async Task RegisterAsync_InsertsUserWithHashedPassword()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);
        
        var dto = new UserDto
        {
            Username = "testuser",
            Password = "password123"
        };
        
        await service.RegisterAsync(dto);
        
        Assert.IsNotNull(mockRepository.InsertedUser);
        Assert.AreEqual(dto.Username, mockRepository.InsertedUser!.Username);
        Assert.IsNotNull(mockRepository.InsertedUser.PasswordHash);
        Assert.IsNotNull(mockRepository.InsertedUser.PasswordSalt);
        Assert.AreNotEqual(System.Text.Encoding.UTF8.GetBytes(dto.Password), mockRepository.InsertedUser.PasswordHash);
    }
    
    [TestMethod]
    public async Task RegisterAsync_ThrowsUserAlreadyExists_WhenUsernameIsTaken()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);

        var dto = new UserDto
        {
            Username = "existinguser",
            Password = "password123"
        };
        
        // Simulate that the user already exists
        mockRepository.UserToReturn = new User
        {
            Id = Guid.NewGuid(),
            Username = "existinguser",
            PasswordHash = new byte[] { },
            PasswordSalt = new byte[] { },
            Rating = 1200
        };
        
        await Assert.ThrowsExceptionAsync<UserAlreadyExists>(async () =>
        {
            await service.RegisterAsync(dto);
        });
    }

    // Login Tests
    [TestMethod]
    public async Task LoginAsync_ReturnsUserId_WhenCredentialsAreValid()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);

        var dto = new UserDto
        {
            Username = "testuser",
            Password = "password123"
        };
        
        // First, register the user to create a hashed password
        await service.RegisterAsync(dto);
        
        // Set the repository to return the inserted user
        mockRepository.UserToReturn = mockRepository.InsertedUser;
        
        var userId = await service.LoginAsync(dto);
        Assert.AreEqual(mockRepository.InsertedUser!.Id, userId);
    }
    
    [TestMethod]
    public async Task LoginAsync_ThrowsUserNotFound_WhenUserDoesNotExist()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);

        var dto = new UserDto
        {
            Username = "nonexistentuser",
            Password = "password123"
        };
        
        await Assert.ThrowsExceptionAsync<UserNotFound>(async () =>
        {
            await service.LoginAsync(dto);
        });
    }
    
    [TestMethod]
    public async Task LoginAsync_ThrowsException_WhenPasswordIsInvalid()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);

        var dto = new UserDto
        {
            Username = "testuser",
            Password = "correctpassword"
        };
        
        // First, register the user to create a hashed password
        await service.RegisterAsync(dto);
        
        // Set the repository to return the inserted user
        mockRepository.UserToReturn = mockRepository.InsertedUser;
        
        var invalidDto = new UserDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };
        
        await Assert.ThrowsExceptionAsync<InvalidCredentials>(async () =>
        {
            await service.LoginAsync(invalidDto);
        });
    }

    [TestMethod]
    public async Task SearchUsersAsync_ReturnsMatchingUsernames_WithMatchingUsername()
    {
        var mockRepository = new MockUserRepository();
        var service = new UserService(mockRepository);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "searchuser",
            PasswordHash = new byte[] { },
            PasswordSalt = new byte[] { },
            Rating = 1200
        };
        
        mockRepository.UserToSearch = user;
        
        var result = await service.SearchUsersAsync("search");
        
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("searchuser", result[0]);
    }
}