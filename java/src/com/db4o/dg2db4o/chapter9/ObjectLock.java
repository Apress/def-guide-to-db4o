
package com.db4o.dg2db4o.chapter9;

import com.db4o.ObjectContainer;

public class ObjectLock {
    private String SEMAPHORE_NAME = "Semaphore: ";
    
    public boolean lock(Object obj, ObjectContainer objectContainer) {
        long id = objectContainer.ext().getID(obj);
        System.out.println("Attempting to get semaphore: " +
                SEMAPHORE_NAME + id);
        return objectContainer.ext().setSemaphore(
                SEMAPHORE_NAME + id, 1000);
    }
    
    public void release(Object obj, ObjectContainer objectContainer) {
        long id = objectContainer.ext().getID(obj);
        System.out.println("Releasing semaphore: " + SEMAPHORE_NAME + id);
        objectContainer.ext().releaseSemaphore(SEMAPHORE_NAME + id);
    }
   
}
