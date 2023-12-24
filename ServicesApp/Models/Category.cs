using System.Security.Cryptography;

namespace ServicesApp.Models
{
	public class Category
	{
        private int _Id;
        private string _Name;
        private string _Description;
        private int _MinFees;

        public Category()
        {
            _Id = 0;
            _Name = string.Empty;
            _Description = string.Empty;
            _MinFees = 0;
        }

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Description 
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public int MinFees
        {
            get { return _MinFees; }
            set { _MinFees = value; }
        }

    }
}
