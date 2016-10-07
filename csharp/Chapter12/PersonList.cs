using System;
using System.Collections;
using System.Text;
using com.db4o;
using com.db4o.types;

namespace com.db4o.dg2db4o.chapter12
{
    class PersonList
    {
        private Db4oList _persons;

        public PersonList()
        {
            ObjectContainer db = Db4o.OpenFile("c:/mixed.yap");
            _persons = db.Ext().Collections().NewLinkedList();
            db.Close();
        }

        public void AddPerson(Person person)
        {
            _persons.Add(person);
        }

        public Db4oList Persons
        {
            get
            {
                return _persons;
            }
        }

    }
}
