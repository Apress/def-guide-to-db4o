
package com.db4o.dg2db4o.chapter7;

import java.util.Date;
import java.text.SimpleDateFormat;
import java.text.ParseException;
import java.util.List;
import java.util.ArrayList;

public class Employee extends AbstractPerson{
    
    int _employeeNumber;
    Date _dob;
    List<Project> _projects;
    
    public Employee(String name, String phoneNumber, String email, int employeeNumber, String dob) {
        super(name,phoneNumber,email);
        _employeeNumber = employeeNumber;
        SimpleDateFormat formatter = new SimpleDateFormat("M/d/yyyy");
        try {
            _dob = formatter.parse(dob);
        } catch (Exception exc) {
            _dob = null;
        }
        _projects = new ArrayList<Project>();
    }
    
    public void sendMail(String fromAddress, String subject, String content) {
        String toAddress = super.getEmail() + "@mycompany.com";
        Emailer mailer = new Emailer();
        mailer.sendMail(fromAddress, toAddress, subject, content);
    }

    public void assignToProject(Project project) {
        _projects.add(project);
        if (!project.employees().contains(this))
            project.assignEmployee(this);
    }

    public int getEmployeeNumber() {
        return _employeeNumber;
    }
    public void setEmployeeNumber(int value) {
        _employeeNumber = value;
    }
    
    public Date getDob() {
        return _dob;
    }
    public void setDoB(Date value) {
        _dob = value;
    }

    public List<Project> projects() {
        return _projects;
    }

    public String toString() {
        return super.getName() + " (Employee)";
    }
    
}
