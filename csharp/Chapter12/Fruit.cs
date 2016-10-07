using System;
using System.Collections.Generic;
using com.db4o;
using com.db4o.ext;

namespace com.db4o.dg2db4o.chapter12
{
    class Fruit
    {
        private string _name;
        private int _amount;
        private IList<String> amountHistory;

        public Fruit(String name, int amount)
        {
            _name = name;
            _amount = amount;
            amountHistory = new List<String>();
        }

        public bool ObjectCanUpdate(ObjectContainer container)
        {
            amountHistory.Add(
                    DateTime.Now.Ticks.ToString()
                    + ", " + _amount);
            Console.WriteLine("I was updated!");
            return false; // Can be updated!
        }

        public override string ToString()
        {
            String ret = "Name=" + _name + "\nAmount=" + _amount;
            foreach (string s in amountHistory)
            {
                ret += "\n Modified at:" + s;
            }
            return ret;
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

        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

    }
}
