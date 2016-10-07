using System;
using System.Collections.Generic;
using System.Text;
using com.db4o.inside.query;

namespace com.db4o.dg2db4o.chapter6
{
    class PersonQueryExecutionHandler 
    {      
        public void NotifyQueryExecuted(object o, QueryExecutionEventArgs e)
        {
            Console.WriteLine(e.ExecutionKind);
        }
    }
}
