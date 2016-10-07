
package com.db4o.dg2db4o.chapter9;

public class Credit implements AccountItem {
    double _amount;
    
    public Credit(double amount) {
        _amount = amount;
    }
    public double getAmount() {
        return _amount;
    }
}
