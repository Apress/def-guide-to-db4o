using System;
using System.Collections;

namespace com.db4o.dg2db4o.chapter7
{
    class Employee : Person
    {
        int _employeeNumber;
        DateTime _dob;
        IList _projects;

        public Employee(string name, string phoneNumber, string email, int employeeNumber, string dob) : base(name,phoneNumber,email)
        {
            _employeeNumber = employeeNumber;
            if(dob!=null) _dob = DateTime.Parse(dob);
            _projects = new ArrayList();
        }

        public override void SendMail(string fromAddress, string subject, string content)
        {
            string toAddress = base.Email + "@mycompany.com";
            System.Net.Mail.SmtpClient mailer = new System.Net.Mail.SmtpClient();
            mailer.Send(fromAddress, toAddress, subject, content);
        }

        public void assignToProject(Project project)
        {
            _projects.Add(project);
            if (!project.Employees.Contains(this))
               project.assignEmployee(this);
        }

        public int EmployeeNumber
        {
            get
            {
                return _employeeNumber;
            }
            set
            {
                _employeeNumber = value;
            }
        }

        public DateTime Dob
        {
            get
            {
                return _dob;
            }
            set
            {
                _dob = value;
            }
        }

        public IList Projects
        {
            get
            {
                return _projects;
            }
        }

        public override string ToString()
        {
            return base.Name + " (Employee)";
        }
    }
}
