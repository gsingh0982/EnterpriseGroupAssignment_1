using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Repository
{
    public interface IEquipmentRepository
    {
        IEnumerable<Equipment> GetAllEquipment();
        Equipment? GetEquipmentById(int id);
        void AddEquipment(Equipment equipment);
    }
}
