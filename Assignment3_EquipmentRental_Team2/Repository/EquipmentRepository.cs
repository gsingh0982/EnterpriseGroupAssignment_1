using Assignment3_EquipmentRental_Team2.Data;
using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _appDbContext;

        public EquipmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddEquipment(Equipment equipment)
        {
            _appDbContext.equipment.Add(equipment);
        }

        public IEnumerable<Equipment> GetAllEquipment()
        {
            return _appDbContext.equipment.ToList();
        }

        public Equipment? GetEquipmentById(int id)
        {
            return _appDbContext.equipment.Find(id);
        }
    }
}
