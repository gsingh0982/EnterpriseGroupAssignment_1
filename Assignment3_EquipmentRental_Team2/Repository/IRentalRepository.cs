using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public interface IRentalRepository
    {
        IEnumerable<RentalInfo> GetAllRentals();
        IEnumerable<RentalInfo> GetRentalsByUserId(int userId);
        RentalInfo? GetRentalById(int id);
        void AddRental(RentalInfo rental);
    }
}
