using Microsoft.EntityFrameworkCore;
using events_tickets_management_backend.Models;
using events_tickets_management_backend.Data;

namespace events_tickets_management_backend.Services
{

    public class TicketService(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<ServiceResponse> GetTicketsInfo(string email, List<string>? seatsIds)
        {
            try
            {
                var tickets = await _context.Tickets.Where(t => 
                        t.SectionNumber == t.SectionNumber &&
                        t.RowNumber == t.RowNumber &&
                        t.SeatNumber == t.SeatNumber &&
                        t.Buyeremail == email).ToListAsync();

                return new SuccessServiceResponse
                {
                    Data = tickets
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw new Exception("An error occurred while processing the transaction.");
            }
        }

        public async Task<ServiceResponse> BuyTicket(string email, List<SeatsInfoType> seatsInfo)
        {
            try
            {
                var seatIdentifiers = seatsInfo.Select(info => new
                {
                    info.EventId,
                    info.RowNumber,
                    info.SectionNumber,
                    info.SeatNumber
                }).ToList();

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var ticketsToBuy = await _context.Tickets
                        .Where(t => seatIdentifiers.Any(s =>
                            s.EventId == t.EventId &&
                            s.RowNumber == t.RowNumber &&
                            s.SectionNumber == t.SectionNumber &&
                            s.SeatNumber == t.SeatNumber))
                        .ToListAsync();

                    if (ticketsToBuy.Any(t => t.Isbought && t.Buyeremail != email))
                    {
                        throw new Exception("One or more tickets are unavailable.");
                    }

                    foreach (var ticket in ticketsToBuy)
                    {
                        ticket.Isbought = true;
                        ticket.Buyeremail = email;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new SuccessServiceResponse
                    {
                        Data = ticketsToBuy
                    };
                }
            }
            catch (Exception ex)
            {
                return new FailServiceResponse
                {
                    Message = "An error occurred while processing the transaction: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponse> RefundTicket(string email, List<SeatsInfoType> seatsInfo)
        {
            try
            {
                var seatIdentifiers = seatsInfo.Select(info => new
                {
                    info.EventId,
                    info.RowNumber,
                    info.SectionNumber,
                    info.SeatNumber
                }).ToList();

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var bookedSeats = await _context.Tickets
                        .Where(t => t.Isbought && t.Buyeremail == email &&
                                     seatIdentifiers.Any(s =>
                                         s.EventId == t.EventId &&
                                         s.RowNumber == t.RowNumber &&
                                         s.SectionNumber == t.SectionNumber &&
                                         s.SeatNumber == t.SeatNumber))
                        .ToListAsync();

                    if (bookedSeats.Count != seatsInfo.Count)
                    {
                        throw new Exception("One or more seats are not booked.");
                    }

                    foreach (var seat in bookedSeats)
                    {
                        seat.Isbought = false;
                        seat.Buyeremail = null;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new SuccessServiceResponse
                    {
                        Message = "Tickets refunded successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new FailServiceResponse
                {
                    Message = "An error occurred while processing the transaction: " + ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
