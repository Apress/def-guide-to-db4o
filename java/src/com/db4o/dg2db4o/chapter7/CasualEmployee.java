
package com.db4o.dg2db4o.chapter7;

public class CasualEmployee extends Employee {
    
    double[] _timeRecord;
        final int MAXTIMERECORDS = 10;
        int numberOfTimeRecords;

        public CasualEmployee(String name, String phoneNumber, String email, int employeeNumber, String dob) 
        {
             super(name, phoneNumber, email, employeeNumber, dob);
            _timeRecord = new double[MAXTIMERECORDS];
            numberOfTimeRecords = 0;

        }

        public double[] getTimeRecord()
        {
                return _timeRecord;
        }
        
        public void setTimeRecord(double[] timeRecord)
        {
                _timeRecord = timeRecord;
        }

        public void addTimeRecord(double newRecord)
        {
            if (numberOfTimeRecords < MAXTIMERECORDS)
            {
                _timeRecord[numberOfTimeRecords] = newRecord;
                numberOfTimeRecords++;
            }
        }

        public String toString()
        {
            return super.getName() + " (CasualEmployee)";
        }
    
}
