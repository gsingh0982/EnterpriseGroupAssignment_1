using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public interface IUserRepository
    {
        User? GetById(int id);
        User? GetByUsername(string username);
        User? GetByProviderSubject(string provider, string subject);
        void AddUser(User user);
    }
}
