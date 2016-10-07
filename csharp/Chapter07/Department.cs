using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter7
{
    class Department
    {
        string _name;
        Manager _manager;

        public Department(string name, Manager manager)
        {
            _name = name;
            _manager = manager;
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

        public Manager Manager
        {
            get
            {
                return _manager;
            }
            set
            {
                _manager = value;
            }
        }

        public override string ToString()
        {
            return Name + " (Department)";
        }
    }
}
