using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter7
{
    class CasualEmployee : Employee
    {
        double[] _timeRecord;
        const int MAXTIMERECORDS = 10;
        int numberOfTimeRecords;


        public CasualEmployee(string name, string phoneNumber, string email, int employeeNumber, string dob) : base(name, phoneNumber, email, employeeNumber, dob)
        {
            _timeRecord = new double[MAXTIMERECORDS];
            numberOfTimeRecords = 0;

        }

        public double[] TimeRecord
        {
            get
            {
                return _timeRecord;
            }
            set
            {
                _timeRecord = value;
            }
        }

        public void addTimeRecord(double newRecord)
        {
            if (numberOfTimeRecords < MAXTIMERECORDS)
            {
                _timeRecord[numberOfTimeRecords] = newRecord;
                numberOfTimeRecords++;
            }
        }

        public override string ToString()
        {
            return base.Name + " (CasualEmployee)";
        }
    }
}
