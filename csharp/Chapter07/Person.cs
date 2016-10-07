

namespace com.db4o.dg2db4o.chapter7
{
    abstract class Person : IPerson
    {
        string _name;
        string _phoneNumber;
        string _email;

        public Person(string name, string phoneNumber, string email)
        {
            _name = name;
            _phoneNumber = phoneNumber;
            _email = email;
        }

        public abstract void SendMail(string fromAddress, string subject, string content);

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
    }
}
