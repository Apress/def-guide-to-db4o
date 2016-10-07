using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;
using com.db4o.replication;

namespace com.db4o.dg2db4o.chapter11
{
    class MyReplicationConflictHandler : ReplicationConflictHandler
    {
        public Object ResolveConflict(
                   ReplicationProcess replicationProcess, Object a, Object b)
        {
            return a;
        }
    }
}
