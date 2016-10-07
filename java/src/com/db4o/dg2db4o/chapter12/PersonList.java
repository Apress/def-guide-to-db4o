/*
 * PersonList.java
 *
 * Created on 02 April 2006, 22:33
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package com.db4o.dg2db4o.chapter12;

import com.db4o.*;
import com.db4o.types.Db4oList;

/**
 *
 * @author jpa1
 */
public class PersonList {
    private Db4oList _persons;
    
     public PersonList()
        {
            ObjectContainer db = Db4o.openFile("c:/mixed.yap");
            _persons = db.ext().collections().newLinkedList();
            db.close();
        }

        public void addPerson(Person person)
        {
            _persons.add(person);
        }

        
        public Db4oList getPersons()
        {
                return _persons;
        }
}
