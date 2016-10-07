
package com.db4o.dg2db4o.chapter7;

public class Address {
    
    String _street;
    String _city;
    String _country;
    private ZipCode _zipCode;
    Customer _customer;
    
    public Address(String street, String city, String country) {
        _street = street;
        _city = city;
        _country = country;
    }
    
    public String getStreet() {
        return _street;
    }
    public void setStreet(String value) {
        _street = value;
    }
    
    public String getCity() {
        return _city;
    }
    public void setCity(String value) {
        _city = value;
    }
    
    public String getCountry() {
        return _country;
    }
    public void setCountry(String value) {
        _country = value;
    }
    
    public Customer getCustomer() {
        return _customer;
    }
    public void setCustomer(Customer value) {
        _customer = value;
    }
    
    public ZipCode getZipCode() {
        return _zipCode;
    }

    public void setZipCode(ZipCode zipCode) {
        this._zipCode = zipCode;
    }
    
    public String toString() {
        return _street + ", " + _city + ", " + _country;
    }
  
}
