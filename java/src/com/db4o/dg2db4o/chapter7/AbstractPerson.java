
package com.db4o.dg2db4o.chapter7;


public abstract class AbstractPerson  implements Person{
    String _name;
    String _phoneNumber;
    String _email;
    
    public AbstractPerson(String name, String phoneNumber, String email) {
        _name = name;
        _phoneNumber = phoneNumber;
        _email = email;
    }
    
    public abstract void sendMail(String fromAddress, String subject, String content);
    
    public String getName() {
        return _name;
    }
    public void setName(String value) {
        _name = value;
    }
    
    public String getPhoneNumber() {
        return _phoneNumber;
    }
    public void setPhoneNumber(String value) {
        _phoneNumber = value;
    }
    
    public String getEmail() {
        return _email;
    }
    public void setEmail(String value) {
        _email = value;
    }
}


