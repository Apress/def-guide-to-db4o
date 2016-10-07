

namespace com.db4o.dg2db4o.chapter7
{
    interface IPerson
    {
        string Name{get; set;}
        string PhoneNumber{get;set;}
        string Email{get;set;}
        void SendMail(string fromAddress, string subject, string content);
    }
}
