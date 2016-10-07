
package com.db4o.dg2db4o.chapter7;

public class Department {
    
    String _name;
    Manager _manager;
    
    public Department(String name, Manager manager) {
        _name = name;
        _manager = manager;
    }
    
    public String getName() {
        return _name;
    }
    public void setName(String value) {
        _name = value;
    }
    
    public Manager getManager() {
        
        return _manager;
    }
    public void setManager(Manager value) {
        _manager = value;
    }
    
    public String toString() {
        return _name + " (Department)";
    }
    
}
