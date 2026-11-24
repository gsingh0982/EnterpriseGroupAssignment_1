using Assignment3_EquipmentRental_Team2.Data;
using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddUser(User user)
        {
            _appDbContext.users.Add(user);
        }

        public User? GetById(int id)
        {
            return _appDbContext.users.Find(id);
        }

        public User? GetByUsername(string username)
        {
            return _appDbContext.users.FirstOrDefault(user => user.Username == username);
        }

        public User? GetByProviderSubject(string provider, string subject)
        {
            return _appDbContext.users.FirstOrDefault(user => user.Provider == provider && user.ProviderSubject == subject);
        }
    }
}
