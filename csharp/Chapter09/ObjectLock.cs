using System;
using com.db4o;

namespace com.db4o.dg2db4o.chapter9
{
        public class ObjectLock
        {
            private string SEMAPHORE_NAME = "Semaphore: ";

            public Boolean Lock(Object obj, ObjectContainer objectContainer)
            {
                long id = objectContainer.Ext().GetID(obj);
                Console.WriteLine("Attempting to get semaphore: " +
                   SEMAPHORE_NAME + id);
                return objectContainer.Ext().SetSemaphore(
                    SEMAPHORE_NAME + id, 1000);
            }

            public void Release(Object obj, ObjectContainer objectContainer)
            {
                long id = objectContainer.Ext().GetID(obj);
                Console.WriteLine("Releasing semaphore: " + SEMAPHORE_NAME + id);
                objectContainer.Ext().ReleaseSemaphore(SEMAPHORE_NAME + id);
            }
        }

}
