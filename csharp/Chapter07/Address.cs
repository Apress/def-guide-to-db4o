
namespace com.db4o.dg2db4o.chapter7
{
    class Address
    {
        string _street;
        string _city;
        string _country;
        ZipCode _zipCode;
       // Customer _customer;

        public Address(string street, string city, string country)
        {
            _street = street;
            _city = city;
            _country = country;
        }

        public string Street
        {
            get
            {
                return _street;
            }
            set
            {
                _street = value;
            }
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
            }
        }

        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
            }
        }

        public ZipCode ZipCode
        {
            get
            {
                return _zipCode;
            }
            set
            {
                _zipCode = value;
            }
        }

        //public Customer Customer
        //{
        //    get
        //    {
        //        return _customer;
        //    }
        //    set
        //    {
        //        _customer = value;
        //    }
        //}

        public override string ToString()
        {
            return _street + ", " + _city + ", " + _country + " (Address)";
        }
    }
}
