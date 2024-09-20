using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using events_tickets_management_backend.Models;
using events_tickets_management_backend.Services;

namespace events_tickets_management_backend.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController(TicketService ticketService) : ControllerBase
    {
        private readonly TicketService _ticketService = ticketService;

        /// <summary>
        /// Retrieve ticket information
        /// </summary>
        /// <param name="email">Email address of the ticket holder.</param>
        /// <param name="seatIds">List of seat IDs to retrieve information for.</param>
        /// <returns>Ticket information</returns>
        [HttpGet]
        public async Task<IActionResult> GetTicketsInfo(string email, List<string> seatIds)
        {
            try
            {
                var response = await _ticketService.GetTicketsInfo(email, seatIds);

                if (response is FailServiceResponse res)
                {
                    return StatusCode(res.StatusCode, res.Message);
                }

                if (response is SuccessServiceResponse resp)
                {
                    return Ok(resp.Data);
                }

                return StatusCode(500, "Internal server error: ");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Purchase tickets
        /// </summary>
        /// <param name="ticketPurchaseDto">Ticket purchase details</param>
        /// <returns>Purchase result</returns>
        [HttpPost("buy")]
        public async Task<IActionResult> BuyTickets([FromBody] TicketPurchaseDto ticketPurchaseDto)
        {
            try
            {
                var response = await _ticketService.BuyTicket(ticketPurchaseDto.Email, ticketPurchaseDto.SeatIds);

                if (response is FailServiceResponse res)
                {
                    return StatusCode(res.StatusCode, res.Message);
                }

                if (response is SuccessServiceResponse resp)
                {
                    return Ok(resp.Data);
                }

                return StatusCode(500, "Internal server error: ");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Refund tickets
        /// </summary>
        /// <param name="ticketRefundDto">Ticket refund details</param>
        /// <returns>Refund result</returns>
        [HttpDelete("refund")]
        public async Task<IActionResult> RefundTickets([FromBody] TicketRefundDto ticketRefundDto)
        {
            try
            {
                var response = await _ticketService.RefundTicket(ticketRefundDto.Email, ticketRefundDto.SeatIds);

                if (response is FailServiceResponse res)
                {
                    return StatusCode(res.StatusCode, res.Message);
                }

                return Ok(new { message = "Refunded successfully" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }

    // DTO classes for request payloads
    public class TicketPurchaseDto
    {
        public required List<SeatsInfoType> SeatIds { get; set; }
        public required string Email { get; set; }
    }

    public class TicketRefundDto
    {
        public required List<SeatsInfoType> SeatIds { get; set; }
        public required string Email { get; set; }
    }
}
