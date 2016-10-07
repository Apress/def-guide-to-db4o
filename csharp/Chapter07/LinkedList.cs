
namespace com.db4o.dg2db4o.chapter7
{
    class LinkedList
    {
        private Employee _item;
        private LinkedList _next;

        public LinkedList()
        {
            _item = null;
            _next = null;
        }

        public LinkedList(Employee item)
        {
            _item = item;
            _next = null;
        }

        public Employee Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }

        public LinkedList Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
            }
        }

        public void append(LinkedList node)
        {
            if (_next == null)
            {
                _next = node;
            }
            else
            {
                _next.append(node);
            }
        }
    }
}
