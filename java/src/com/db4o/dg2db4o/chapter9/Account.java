
package com.db4o.dg2db4o.chapter9;

import java.util.List;
import java.util.ArrayList;

public class Account {
    List<AccountItem> _accountItems;
    
    public Account(){
        _accountItems = new ArrayList<AccountItem>();
    }  
    
    public void credit(double amount) {
        _accountItems.add(new Credit(amount));
    }
    
    public void debit(double amount) throws NegativeBalanceException {
        _accountItems.add(new Debit(amount));
        if (balance() < 0.0) {
            throw new NegativeBalanceException(balance());
        }
    }
    
    public double balance() {
        double balance = 0.0;
        for(AccountItem accountItem : _accountItems){
            balance += accountItem.getAmount();
        }
        return balance;
    }
}
