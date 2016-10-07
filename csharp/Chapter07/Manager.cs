using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter7
{
    class Manager : Employee
    {
        LinkedList _employees;

        public Manager(string name, string phoneNumber, string email, int employeeNumber, string dob) : base(name, phoneNumber, email, employeeNumber, dob)
        {
            //_employees = new List<Employee>();    
        }

        public LinkedList Employees
        {
            get
            {
                return _employees;
            }
        }

        public void addEmployee(Employee newEmployee)
        {
            LinkedList newNode = new LinkedList(newEmployee);
            if (_employees == null)
            {
                _employees = newNode;
            }
            else
            {
                _employees.append(newNode);
            }
            //_employees.Add(newEmployee);
        }

        public override string ToString()
        {
            return base.Name + " (Manager)";
        }
    }
}
