namespace events_tickets_management_backend.Models
{
    public abstract class ServiceResponse
    {
        public bool Success { get; protected set; }
        public string? Message { get; set; }
    }

    public class SuccessServiceResponse : ServiceResponse
    {
        public object? Data { get; set; }

        public SuccessServiceResponse()
        {
            Success = true;
        }
        public new string? Message
        {
            get => base.Message;
            set => base.Message = value;
        }
    }

    // public class SuccessServiceResponseWithData<T>(T data) : SuccessServiceResponse
    // {
    //     public T Data { get; set; } = data;
    // }

    public class FailServiceResponse : ServiceResponse
    {
        public FailServiceResponse()
        {
            Success = false;
        }

        public new string? Message
        {
            get => base.Message;
            set => base.Message = value;
        }

        public int StatusCode { get; set; }
    }

    public class SeatsInfoType
    {
        public int EventId { get; set; }
        public int SeatNumber { get; set; }
        public int RowNumber { get; set; }
        public int SectionNumber { get; set; }
    }
}