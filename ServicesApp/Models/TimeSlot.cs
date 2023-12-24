using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace ServicesApp.Models
{
    public class TimeSlot
    {
        private int? _Id;
        private DateOnly? _Date;
        private TimeOnly? _FromTime;
        private TimeOnly? _ToTime;
        private ServiceRequest? _ServiceRequest;


        public int? Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public DateOnly? Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
        public TimeOnly? FromTime
        {
            get { return _FromTime; }
            set { _FromTime = value; }
        }
        public TimeOnly? ToTime
        {
            get { return _ToTime; }
            set { _ToTime = value; }
        }
        public ServiceRequest? ServiceRequest
        {
            get { return _ServiceRequest; }
            set { _ServiceRequest = value; }
        }
    }
}
