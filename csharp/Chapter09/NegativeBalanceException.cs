using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter9
{
    class NegativeBalanceException : Exception
    {
        double _balance;

        public NegativeBalanceException(double balance)
        {
            _balance = balance;
        }

        public override string ToString()
        {   
            return "NegativeBalanceException (Balance = " + System.String.Format("{0:C}",_balance) + ")";
        }
    }
}
