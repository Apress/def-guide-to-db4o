using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace com.db4o.dg2db4o.chapter7
{
    class Project
    {
        string _name;
        string _costCode;
        //IList<Employee> _employees;
        IList _employees;

         public Project(string name, string costCode)
        {
            _name = name;
            _costCode = costCode;
            //_employees = new List<Employee>();  
            _employees = new ArrayList();  

        }

        //public Project(string name, string costCode, IList<Employee> employees)
        public Project(string name, string costCode, IList employees)
        {
            _name = name;
            _costCode = costCode;
            _employees = employees;
        }

        public void assignEmployee(Employee newEmployee)
        {
            _employees.Add(newEmployee);
           if(!newEmployee.Projects.Contains(this))
               newEmployee.assignToProject(this);
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string CostCode
        {
            get
            {
                return _costCode;
            }
            set
            {
                _costCode = value;
            }
        }

        //public IList<Employee> Employees
        public IList Employees
        {
            get
            {
                return _employees;
            }
        }

        public override string ToString()
        {
            return _name + " (Project)";
        }
    }
}
