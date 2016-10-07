using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter9
{
    class Debit : IAccountItem
    {
         double _amount;

        public Debit(double amount)
        {
            _amount = amount;
        }

        public double Amount
        {
            get
            {
                return -(_amount);
            }
        }
    }
}
