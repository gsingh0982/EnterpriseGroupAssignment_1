using Assignment3_EquipmentRental_Team2.Data;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public IEquipmentRepository Equipment { get; set; }
        public IRentalRepository Rentals { get; set; }
        public IUserRepository Users { get; set; }

        public UnitOfWork(AppDbContext appDbContext, IEquipmentRepository equipment,
                          IRentalRepository rentals, IUserRepository users)
        {
            _appDbContext = appDbContext;
            Equipment = equipment;
            Rentals = rentals;
            Users = users;
        }

        public int Complete()
        {
            return _appDbContext.SaveChanges();
        }
    }
}
