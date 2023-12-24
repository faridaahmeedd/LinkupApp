using ServicesApp.Core.Models;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace ServicesApp.Models
{
    public class ServiceRequest
	{
        private int? _Id;
        private string? _Description;
        private int? _Fees;
        private byte[]? _Image;
        private Category? _Category;
		private string? _Status;
        private Customer? _Customer;
        private ICollection<ServiceOffer>? _Offers;
        private ICollection<TimeSlot>? _TimeSlots;


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
        public string? Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public byte[]? Image
        {
            get { return _Image; }
            set { _Image  = value; }
        }
        public Category? Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        public string? Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public Customer? Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }
        public ICollection<ServiceOffer>? Offers
        {
            get { return _Offers; }
            set { _Offers = value; }
        }
        public ICollection<TimeSlot>? TimeSlots
        {
            get { return _TimeSlots; }
            set { _TimeSlots = value; }
        }

    }
}