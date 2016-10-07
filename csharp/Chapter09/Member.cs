

namespace com.db4o.dg2db4o.chapter9
{
    class Member : Person
    {
        Account _account;
        
        public Member(string name, string phoneNumber, string email) : base(name,phoneNumber,email)
        {
            _account = new Account();
        }

        public Account Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
            }
        }

        public override string ToString()
        {
            return base.Name + " (Member): " + System.String.Format("{0:C}", _account.Balance()); 
        }
    }
}
