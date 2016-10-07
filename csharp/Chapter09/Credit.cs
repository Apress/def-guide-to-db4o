

namespace com.db4o.dg2db4o.chapter9
{
    class Credit : IAccountItem
    {
        double _amount;

        public Credit(double amount)
        {
            _amount = amount;
        }

        public double Amount
        {
            get
            {
                return _amount;
            }
        }
    }
}
