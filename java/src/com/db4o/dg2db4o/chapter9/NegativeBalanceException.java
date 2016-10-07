
package com.db4o.dg2db4o.chapter9;

import java.text.NumberFormat;

public class NegativeBalanceException extends Exception
{
    double _balance;

        public NegativeBalanceException(double balance)
        {
            _balance = balance;
        }

        public String toString()
        {   
            return "NegativeBalanceException (Balance = " + NumberFormat.getCurrencyInstance().format(_balance) + ")";
        }
}
