using Assignment3_EquipmentRental_Team2.Models;
using Assignment3_EquipmentRental_Team2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_EquipmentRental_Team2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // every endpoint needs a valid application JWT
    public class EquipmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET /api/equipment  (any signed-in user)
        [HttpGet]
        public ActionResult<IEnumerable<Equipment>> GetEquipment()
        {
            return Ok(_unitOfWork.Equipment.GetAllEquipment());
        }

        // POST /api/equipment  (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<Equipment> CreateEquipment(Equipment equipment)
        {
            if (string.IsNullOrWhiteSpace(equipment.Name))
            {
                return BadRequest(new { message = "Name is required" });
            }

            // Fields the client must not choose
            equipment.Id = 0;
            equipment.IsAvailable = true;

            _unitOfWork.Equipment.AddEquipment(equipment);
            _unitOfWork.Complete();

            return Ok(equipment);
        }
    }
}
