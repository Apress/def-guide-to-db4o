

package com.db4o.dg2db4o.chapter7;

import java.util.List;
//import java.util.LinkedList;   // if using instead of simple LinkedList

public class Manager extends Employee {
    
    LinkedList _employees;

        public Manager(String name, String phoneNumber, String email, int employeeNumber, String dob)
        {
            super(name, phoneNumber, email, employeeNumber, dob);  
        }

        public LinkedList employees()
        {
                return _employees;
        }

        public void addEmployee(Employee newEmployee)
        {
            LinkedList newNode = new LinkedList(newEmployee);
            if(_employees==null){
                _employees = newNode;
            }
            else {
            _employees.append(newNode);
            }
            //_employees.addLast(newEmployee);   // if using java.util.LinkedList
        }

        public String toString()
        {
            return super.getName() + " (Manager)";
        }
    
}
