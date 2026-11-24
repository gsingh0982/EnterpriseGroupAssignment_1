using System.Security.Claims;
using Assignment3_EquipmentRental_Team2.Models;
using Assignment3_EquipmentRental_Team2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_EquipmentRental_Team2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // every endpoint needs a valid application JWT
    public class RentalsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentalsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // The signed-in user id comes from the JWT, never from the request body
        private int CurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // GET /api/rentals/mine  (any signed-in user: ONLY their own rentals)
        [HttpGet("mine")]
        public ActionResult<IEnumerable<RentalInfo>> GetMyRentals()
        {
            return Ok(_unitOfWork.Rentals.GetRentalsByUserId(CurrentUserId()));
        }

        // POST /api/rentals  (any signed-in user; the client only sends equipmentId)
        [HttpPost]
        public ActionResult<RentalInfo> CreateRental(RentalInfo rental)
        {
            var equipment = _unitOfWork.Equipment.GetEquipmentById(rental.EquipmentId);
            if (equipment == null)
            {
                return NotFound(new { message = "Equipment not found" });
            }
            if (!equipment.IsAvailable)
            {
                return BadRequest(new { message = "Equipment is not available" });
            }

            var userId = CurrentUserId();
            var user = _unitOfWork.Users.GetById(userId);

            // Overwrite everything the client must NOT control
            rental.Id = 0;
            rental.UserId = userId;
            rental.UserName = user?.Name ?? user?.Username ?? "Unknown";
            rental.EquipmentName = equipment.Name;
            rental.Status = "Pending";
            rental.RequestedAt = DateTime.UtcNow;

            _unitOfWork.Rentals.AddRental(rental);
            _unitOfWork.Complete();

            return Ok(rental);
        }

        // GET /api/rentals  (Admin only: every rental)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<RentalInfo>> GetAllRentals()
        {
            return Ok(_unitOfWork.Rentals.GetAllRentals());
        }

        // POST /api/rentals/{id}/approve  (Admin only)
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public ActionResult<RentalInfo> Approve(int id)
        {
            return Decide(id, "Approved");
        }

        // POST /api/rentals/{id}/reject  (Admin only)
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public ActionResult<RentalInfo> Reject(int id)
        {
            return Decide(id, "Rejected");
        }

        private ActionResult<RentalInfo> Decide(int id, string status)
        {
            var rental = _unitOfWork.Rentals.GetRentalById(id);
            if (rental == null)
            {
                return NotFound(new { message = "Rental not found" });
            }
            if (rental.Status != "Pending")
            {
                return BadRequest(new { message = "Only pending requests can be approved or rejected" });
            }

            rental.Status = status;
            _unitOfWork.Complete();

            return Ok(rental);
        }
    }
}
