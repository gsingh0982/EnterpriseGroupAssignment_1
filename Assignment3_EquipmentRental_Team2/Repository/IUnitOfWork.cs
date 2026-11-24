namespace Assignment3_EquipmentRental_Team2.Repository
{
    public interface IUnitOfWork
    {
        IEquipmentRepository Equipment { get; }
        IRentalRepository Rentals { get; }
        IUserRepository Users { get; }

        int Complete();
    }
}
