
package com.db4o.dg2db4o.chapter9;

import java.text.NumberFormat;

public class Member extends AbstractPerson {
    Account _account;
    
    public Member(String name, String phoneNumber, String email) {
        super(name,phoneNumber,email);
        _account = new Account();
    }
    
    public Account getAccount(){
        return _account;
    }
    
    public void setAccount(Account value) {
        _account = value;
    }
    
    public String toString() {
        return _name + " (Member): " + NumberFormat.getCurrencyInstance().format(_account.balance());
    }
}
