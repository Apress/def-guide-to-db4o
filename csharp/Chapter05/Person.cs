using System;

namespace com.db4o.dg2db4o.chapter5
{
    public class Person
    {
        private string _name;
        private int _age;

        public Person() { }

        public Person(String name, int age)
        {
            _name = name;
            _age = age;
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

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
            }
        }

        public override string ToString()
        {
            return "[" + Name + ";" + Age + "]";
        }
    }
}
