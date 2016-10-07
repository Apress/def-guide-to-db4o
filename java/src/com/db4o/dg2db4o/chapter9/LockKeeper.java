
package com.db4o.dg2db4o.chapter9;

import com.db4o.*;

public class LockKeeper {
    
    private String SEMAPHORE_NAME = "Semaphore: ";
    private ObjectContainer _objectContainer;
    
    public LockKeeper(ObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }
    
    public boolean lock(Object obj)
    {
        long id = _objectContainer.ext().getID(obj);
        System.out.println("Attempting to get semaphore: " + SEMAPHORE_NAME + id);
        return _objectContainer.ext().setSemaphore(SEMAPHORE_NAME + id, 1000);
    }
    
    public void unLock(Object obj)
    {
        long id = _objectContainer.ext().getID(obj);
        System.out.println ("Releasing semaphore: " + SEMAPHORE_NAME + id);
        _objectContainer.ext().releaseSemaphore(SEMAPHORE_NAME + id);
    }

    
}
