
using System.Collections.Generic;

namespace com.db4o.dg2db4o.chapter9
{
    class Account 
    {
        IList<IAccountItem> _accountItems;

        public Account(){
            _accountItems = new List<IAccountItem>();
        }


        public void Credit(double amount)
        {
            _accountItems.Add(new Credit(amount));
        }

        public void Debit(double amount)
        {
            _accountItems.Add(new Debit(amount));
            if (Balance() < 0.0)
            {
                throw new NegativeBalanceException(Balance());
            }
        }
        
        public double Balance() 
        {
            double balance = 0.0;
            foreach(IAccountItem accountItem in _accountItems){
                balance += accountItem.Amount;
            }
            return balance;
        }
    }
}
