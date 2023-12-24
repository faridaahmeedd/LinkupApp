using System.Security.Cryptography;

namespace ServicesApp.Models
{
    public class ServiceOffer
	{
        private int? _Id;
        private int? _Fees;
        private bool? _Accepted = false;
        private int? _TimeSlotId;
        private Provider? _Provider;
        private ServiceRequest? _Request;


        public int? Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public int? Fees
        {
            get { return _Fees; }
            set { _Fees = value; }
        }
        public bool? Accepted
        {
           get { return _Accepted; }
           set { _Accepted = value; }
        }
        public int? TimeSlotId
        {
            get { return _TimeSlotId; } 
            set { _TimeSlotId = value; }
        }

        public Provider? Provider
        {
            get { return _Provider; }
            set { _Provider = value; }
        }
        public ServiceRequest? Request
        {
            get { return _Request; }
            set { _Request = value; }
        }

    }
}