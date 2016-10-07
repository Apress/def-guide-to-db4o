
package com.db4o.dg2db4o.chapter7;

public class Customer extends AbstractPerson{
    
    //static String _classVar = "CLASSVAR";
    Address _address;
    int _age;
    
    public Customer(String name, String phoneNumber, String email, Address address, int age) {
        super(name,phoneNumber,email);
        _address = address;
        _age = age;
        address.setCustomer(this);
    }
    
    public void sendMail(String fromAddress, String subject, String content) {
        String toAddress = super.getEmail();
        String sentContent = content + "COMPANY DISCLAIMER...";
        Emailer mailer = new Emailer();
        mailer.sendMail(fromAddress, toAddress, subject, content);
    }
    
    public Address getAddress() {
        return _address;
    }
    public void setAddress(Address value) {
        _address = value;
        value.setCustomer(this);
    }
    
    public String toString() {
        return super.getName() + " (Customer)";
    }

   
}
