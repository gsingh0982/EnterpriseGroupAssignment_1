using Assignment3_EquipmentRental_Team2.Data;
using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public class RentalRepository : IRentalRepository
    {
        private readonly AppDbContext _appDbContext;

        public RentalRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddRental(RentalInfo rental)
        {
            _appDbContext.rentals.Add(rental);
        }

        public IEnumerable<RentalInfo> GetAllRentals()
        {
            return _appDbContext.rentals.ToList();
        }

        public IEnumerable<RentalInfo> GetRentalsByUserId(int userId)
        {
            return _appDbContext.rentals.Where(rental => rental.UserId == userId).ToList();
        }

        public RentalInfo? GetRentalById(int id)
        {
            return _appDbContext.rentals.Find(id);
        }
    }
}
