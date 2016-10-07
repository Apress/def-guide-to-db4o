

namespace com.db4o.dg2db4o.chapter7
{
    class Customer : Person
    {
        Address _address;
        
        public Customer(string name, string phoneNumber, string email, Address address) : base(name,phoneNumber,email)
        {
            _address = address;
    //        address.Customer = this;
        }

        public override void SendMail(string fromAddress, string subject, string content)
        {
            string toAddress = base.Email;
            string sentContent = content + "COMPANY DISCLAIMER...";
            System.Net.Mail.SmtpClient mailer = new System.Net.Mail.SmtpClient();
            mailer.Send(fromAddress, toAddress, subject, content);
        }

        public Address Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                //value.Customer = this;
            }
        }

        public override string ToString()
        {
            return base.Name + " (Customer)";
        }
    }
}
