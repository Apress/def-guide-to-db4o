using System;
using com.db4o;
using com.db4o.ext;

namespace com.db4o.dg2db4o.chapter9
{
  public class LockManager {
    
    private string SEMAPHORE_NAME = "locked: ";
    private int WAIT_FOR_AVAILABILITY = 300; // 300 milliseconds
    private ExtObjectContainer _objectContainer;
    
    public LockManager(ObjectContainer objectContainer){
        _objectContainer = objectContainer.Ext();
    }
    
    public Boolean Lock(Object obj){
        long id = _objectContainer.GetID(obj);
        Console.WriteLine("Attempting to get semaphore: " + SEMAPHORE_NAME + id);
        return _objectContainer.SetSemaphore(SEMAPHORE_NAME + id, WAIT_FOR_AVAILABILITY);
    }
    
    public void UnLock(Object obj){
        long id = _objectContainer.GetID(obj);
        Console.WriteLine("Releasing semaphore: " + SEMAPHORE_NAME + id);
        _objectContainer.ReleaseSemaphore(SEMAPHORE_NAME + id);
    }
}

}
