/*
 * ObjectDAO.java
 *
 */

package com.db4o.dg2db4o.chapter7;

import com.db4o.*;

public class ObjectDAO {
    
String _fileName;

        public ObjectDAO(String fileName)
        {
            _fileName = fileName;
        }

        public void StoreObject(Object o)
        {
            ObjectContainer db = Db4o.openFile(_fileName);
            try
            {
                db.set(o);

            }
            finally
            {
                db.close();
            }
        }
    
}
