
package com.db4o.dg2db4o.chapter7;

import java.util.List;
import java.util.ArrayList;

public class Project {
    
    String _name;
    String _costCode;
    List<Employee> _employees;
    
    public Project(String name, String costCode) {
        _name = name;
        _costCode = costCode;
        _employees = new ArrayList<Employee>();
    }
    
    public Project(String name, String costCode, List<Employee> employees) {
        _name = name;
        _costCode = costCode;
        _employees = employees;
    }
    
    public void assignEmployee(Employee newEmployee) {
        _employees.add(newEmployee);
        if(!newEmployee.projects().contains(this))
            newEmployee.assignToProject(this);
    }
    
    public String getName() {
        return _name;
    }
    
    public void setName(String value) {
        _name = value;
    }
    
    public String getCostCode() {
        return _costCode;
    }
    
    public void setCostCode(String value){
        _costCode = value;
    }
    
    public List<Employee> employees() {
        return _employees;
    }
    
    public String toString() {
        return _name + " (Project)";
    }
    
}
