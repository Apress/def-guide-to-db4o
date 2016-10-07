
package com.db4o.dg2db4o.chapter9;

public class Debit implements AccountItem {
    double _amount;
    
    public Debit(double amount) {
        _amount = amount;
    }
    
    public double getAmount() {
        return -(_amount);
    }
}
