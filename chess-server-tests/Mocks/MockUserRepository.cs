using chess_server.Models;
using chess_server.Repositories;

namespace chess_server_tests.Mocks;

public class MockUserRepository : IUserRepository
    {
        public User? InsertedUser { get; private set; }
        public User? UserToSearch { get; set; }
        public User? UserToReturn { get; set; }

        public Task InsertUserAsync(User user)
        {
            InsertedUser = user;
            return Task.CompletedTask;
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(UserToReturn);
        }
        
        public Task<List<User>> SearchUsersByUsernameAsync(string username)
        {
            var result = new List<User>();
            if (UserToSearch != null && UserToSearch.Username.Contains(username))
            {
                result.Add(UserToSearch);
            }
            return Task.FromResult(result);
        }
    }
