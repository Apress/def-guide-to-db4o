

package com.db4o.dg2db4o.chapter7;

import java.io.File;
import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import com.db4o.*;
import com.db4o.query.*;

public class Main {
    private static final String DBFILE = "C:/objects.yap";
    
    public static void main(String[] args) {
        simpleStructuredObjects();
        hierarchy();
        inheritance();
        arrays();
        collections();
        collectionsActivation();
        deepGraph();
        composite();
    }
    
    private static void resetDatabase() {
        new File(DBFILE).delete();
    }
    
    private static void simpleStructuredObjects(){
        // Declarations for entities which will be reused several times in this method
        ObjectContainer db;
        List<Customer> customers;
        List<Address> addresses;
        Customer customerExample;
        Address addressExample;
        Customer cu;
        ObjectSet results;
        
        System.out.println("\nSIMPLE STRUCTURED OBJECTS");
        
        resetDatabase();
        
        // Create test objects
        Address a1 = new Address("1 First Street", "San Jose", "USA");
        Customer cu1 = new Customer("Gary", "408 123 4567", "gary@someisp.com", a1, 25);
        
        // Store Customer (or Address)
        db = Db4o.openFile(DBFILE);
        db.set(cu1);
        //db.set(a1);
        db.close();
        
        // Retrieve and display Customer - QBE
        db = Db4o.openFile(DBFILE);
        customerExample = new Customer("Gary", null, null, new Address(null,null,null),0);
        results = db.get(customerExample);
        System.out.println("CUSTOMER QBE");
        while (results.hasNext()) {
            Customer customer = (Customer)results.next();
            System.out.println(customer);
            System.out.println(customer.getAddress());
        }
        db.close();
        
        // Retrieve and display Customer - Native Query
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust) {
                return cust.getName().equals("Gary");
            }
        });
        System.out.println("CUSTOMER NQ");
        for (Customer customer : customers ) {
            System.out.println(customer);
            System.out.println(customer.getAddress());
        }
        db.close();
        
        // Retrieve and display Address - QBE
        db = Db4o.openFile(DBFILE);
        addressExample = new Address("1 First Street", null, null);
        results = db.get(addressExample);
        System.out.println("ADDRESS QBE");
        while (results.hasNext()) {
            Address address = (Address)results.next();
            System.out.println(address);
        }
        db.close();
        
        // Retrieve and display Address - Native Query
        db = Db4o.openFile(DBFILE);
        addresses = db.query(new Predicate<Address>() {
            public boolean match(Address add){
                return add.getStreet().equals("1 First Street");
            }
        });
        System.out.println("ADDRESS NQ");
        for (Address address : addresses ) {
            System.out.println(address);
            //         System.out.println(address.getCustomer());
        }
        db.close();
        
        // Retrieve and display Customer by Address - Native Query
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust){
                return cust.getAddress().getStreet().equals("1 First Street");
            }
        });
        System.out.println("CUSTOMER BY ADDRESS NQ");
        for (Customer customer : customers){
            System.out.println(customer);
            System.out.println(customer.getAddress());
        }
        db.close();
        
        // Retrieve and display Customer by Address - S.O.D.A.
        db = Db4o.openFile(DBFILE);
        Query query = db.query();
        query.constrain(Customer.class);
        query.descend("_address").descend("_street").constrain("1 First Street");
        results = query.execute();
        System.out.println("CUSTOMER BY ADDRESS SODA");
        while (results.hasNext()) {
            Customer customer = (Customer)results.next();
            System.out.println(customer);
            System.out.println(customer.getAddress());
        }
        db.close();
        
        // Retrieve Customer and update Address
        Db4o.configure().objectClass(Customer.class).cascadeOnUpdate(true);
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust) {
                return cust.getName().equals("Gary");
            }
        });
        cu = customers.get(0);  // get first returned Customer - should only be one
        cu.getAddress().setStreet("2 Second Street");
        db.set(cu);
        db.close();
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust) {
                return cust.getName().equals("Gary");
            }
        });
        System.out.println("CUSTOMER AFTER UPDATE");
        for (Customer customer : customers){
            System.out.println(customer);
            System.out.println(customer.getAddress());
        }
        db.close();
        
        // Test activation
        Db4o.configure().activationDepth(1);
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust) {
                return cust.getName().equals("Gary");
            }
        });
        cu = customers.get(0);  // get first returned Customer - should only be one
        System.out.println("ACTIVATION - no output, set breakpoints");
        db.activate(cu.getAddress(), 1);
        db.close();
        Db4o.configure().activationDepth(5);
        
        // Delete Customer and test cascading
        Db4o.configure().objectClass(Customer.class).cascadeOnDelete(true);
        db = Db4o.openFile(DBFILE);
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust) {
                return cust.getName().equals("Gary");
            }
        });
        cu = customers.get(0);  // get first returned Customer - should only be one
        db.delete(cu);
        db.close();
        db = Db4o.openFile(DBFILE);
        addresses = db.query(new Predicate<Address>() {
            public boolean match(Address add){
                return add.getStreet().equals("2 Second Street");
            }
        });
        System.out.println("ADDRESS AFTER CUSTOMER DELETED");
        for (Address address : addresses ) {
            System.out.println(address);
        }
        db.close();
        
        Db4o.configure().activationDepth(5);   // reset to default value
        
        System.out.println("\nEND SIMPLE STRUCTURED OBJECTS");
    }
    
    private static void hierarchy() {
        ObjectContainer db;
        List<Customer> customers;
        List<Address> addresses;
        Customer customerExample;
        Address addressExample;
        ZipCode zipExample;
        ObjectSet results;
        Customer cu;
        
        System.out.println("\nOBJECT HIERARCHY");
        
        resetDatabase();
        
        // Create test objects
        ZipCode z1 = new ZipCode("CA", "95200", "1234");
        ZipCode z2 = new ZipCode("CA", "95200", "5678");
        Address a1 = new Address("1 First Street", "San Jose", "USA");
        a1.setZipCode(z1);
        Address a2 = new Address("2 Second Street", "San Jose", "USA");
        a2.setZipCode(z2);
        Customer cu1 = new Customer("Gary", "408 123 4567", "gary@example.net", a1, 25);
        Customer cu2 = new Customer("Mary", "408 101 1001", "mary@example.com", a2, 24);
        
        // Store Customers
        db = Db4o.openFile(DBFILE);
        db.set(cu1);
        db.set(cu2);
        db.close();
        
        // Retrieve and display Customer by ZipCode - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("CUSTOMER BY ZIPCODE - QBE");
        zipExample = new ZipCode(null, null, "5678");
        addressExample = new Address(null,null,null);
        addressExample.setZipCode(zipExample);
        customerExample = new Customer(null, null, null, addressExample,0);
        results = db.get(customerExample);
        while (results.hasNext()) {
            Customer customer = (Customer)results.next();
            System.out.println(customer);
            System.out.println(customer.getAddress());
            System.out.println(customer.getAddress().getZipCode());
        }
        db.close();
        
        // Retrieve and display Customer by ZipCode - Native Query
        db = Db4o.openFile(DBFILE);
        System.out.println("CUSTOMER BY ZIPCODE - NQ");
        customers = db.query(new Predicate<Customer>() {
            public boolean match(Customer cust){
                return cust.getAddress().getZipCode().getExtension().equals("1234");
            }
        });
        for(Customer customer : customers) {
            System.out.println(customer);
            System.out.println(customer.getAddress());
            System.out.println(customer.getAddress().getZipCode());
        }
        db.close();
        
        // Retrieve and display Customer by ZipCode - SODA
        db = Db4o.openFile(DBFILE);
        System.out.println("CUSTOMER BY ZIPCODE - SODA");
        Query query = db.query();
        query.constrain(Customer.class);
        //query.Descend("_address").Descend("_zipCode").Descend("_extension").Constrain("5678");  // alternative version
        Query zipQuery = query.descend("_address").descend("_zipCode");
        zipQuery.descend("_extension").constrain("5678");
        results = query.execute();
        while (results.hasNext()) {
            Customer customer = (Customer)results.next();
            System.out.println(customer);
            System.out.println(customer.getAddress());
            System.out.println(customer.getAddress().getZipCode());
        }
        db.close();
        
        System.out.println("\nEND OBJECT HIERARCHY");
        
        
    }
    
    private static void inheritance() {
        ObjectContainer db;
        ObjectSet results;
        
        System.out.println("\nINHERITANCE");
        
        resetDatabase();
        
        // Create test objects
        Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
        Manager m1 = new Manager("Sue", "9876", "sue", 102, "3/8/1982");
        CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 103, "7/6/1986");
        //     Customer cu1 = new Customer("Gary", "408 123 4567", "gary@someisp.com", new Address("1 First Street", "San Jose", "USA"));
        
        // Store objects
        db = Db4o.openFile(DBFILE);
        db.set(e1);
        db.set(m1);
        db.set(c1);
        //      db.set(cu1);
        db.close();
        
        // Query for Employee objects - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("GET EMPLOYEES - QBE");
        Employee employeeExample = new Employee(null, null, null, 0, null);
        results = db.get(employeeExample);
        listObjectSet(results);
        db.close();
        
        // Query for Manager objects - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("GET MANAGERS - QBE");
        Manager managerExample = new Manager(null, null, null, 0, null);
        results = db.get(managerExample);
        listObjectSet(results);
        db.close();
        
        // Query for Person objects - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("GET ABSTRACTPERSONS - QBE");
        results = db.get(AbstractPerson.class);
        listObjectSet(results);
        db.close();
        
        // Query for IPerson objects - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("GET PERSONS (INTERFACE)- QBE");
        results = db.get(Person.class);
        listObjectSet(results);
        db.close();
        
        System.out.println("\nEND INHERITANCE");
    }
    
    private static void arrays() {
        ObjectContainer db;
        ObjectSet results;
        CasualEmployee casEmpExample;
        CasualEmployee ce;
        List<CasualEmployee> casEmps;
        
        System.out.println("\nARRAYS");
        
        resetDatabase();
        
        // Create test objects
        CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 101, "7/6/1986");
        CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 102, "11/3/1984");
        c1.addTimeRecord(2.5);
        c1.addTimeRecord(4.0);
        c1.addTimeRecord(1.5);
        c1.addTimeRecord(2.0);
        c2.addTimeRecord(3.5);
        c2.addTimeRecord(4.0);
        c2.addTimeRecord(5.0);
        
        // Store objects
        db = Db4o.openFile(DBFILE);
        db.set(c1);
        db.set(c2);
        db.close();
        
        // Query for CasualEmployee objects (contains 4.0) - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (CONTAINS 4.0) - QBE");
        casEmpExample = new CasualEmployee(null, null, null, 0, null);
        casEmpExample.setTimeRecord(new double[]{4.0});
        results = db.get(casEmpExample);
        listObjectSet(results);
        db.close();
        
        // Query for CasualEmployee objects (contains 2.5 & 4.0) - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (CONTAINS 2.5 AND 4.0) - QBE");
        casEmpExample = new CasualEmployee(null, null, null, 0, null);
        casEmpExample.setTimeRecord(new double[]{2.5, 4.0});
        results = db.get(casEmpExample);
        listObjectSet(results);
        db.close();
        
        // Query for CasualEmployee objects (contains 3.5 & 5.0) - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (CONTAINS 3.5 AND 5.0) - QBE");
        casEmpExample = new CasualEmployee(null, null, null, 0, null);
        casEmpExample.setTimeRecord(new double[]{3.5, 5.0});
        results = db.get(casEmpExample);
        listObjectSet(results);
        db.close();
        
        // Try to query for array directly
        db = Db4o.openFile(DBFILE);
        System.out.println("QUERY FOR ARRAY DIRECTLY (CONTAINS 2.5 AND 4.0) - QBE");
        results = db.get(new double[]{ 2.5, 4.0 });
        listObjectSet(results);
        db.close();
        
        // Query for CasualEmployee objects (contains 4.0) - Native Query
        db = Db4o.openFile(DBFILE);
        
        System.out.println("CASUALEMPLOYEE (CONTAINS 4.0) - NQ");
        casEmps = db.query(new Predicate<CasualEmployee>() {
            public boolean match(CasualEmployee casEmp) {
                Arrays.sort(casEmp.getTimeRecord());  // sort for binary search
                return Arrays.binarySearch(casEmp.getTimeRecord(),4.0) >= 0;
            }
        });
        for (CasualEmployee casEmp : casEmps){
            System.out.println(casEmp);
        }
        db.close();
        
        // Query for CasualEmployee objects (contains 2.5 & 4.0) - Native Query
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (CONTAINS 2.5 & 4.0) - NQ");
        casEmps = db.query(new Predicate<CasualEmployee>() {
            public boolean match(CasualEmployee casEmp) {
                Arrays.sort(casEmp.getTimeRecord());  // sort for binary search
                return Arrays.binarySearch(casEmp.getTimeRecord(),2.5) >= 0 &&
                        Arrays.binarySearch(casEmp.getTimeRecord(),4.0) >= 0;
            }
        });
        for (CasualEmployee casEmp : casEmps){
            System.out.println(casEmp);
        }
        db.close();
        
        // Query for CasualEmployee objects (> 3 TimeRecords) - Native Query
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (> 3 TIMERECORDS) - NQ");
        casEmps = db.query(new Predicate<CasualEmployee>() {
            public boolean match(CasualEmployee casEmp) {
                double totalTime = 0.0;
                int numberOfRecords = 0;
                while ((casEmp.getTimeRecord()[numberOfRecords] != 0) &&
                        (numberOfRecords < casEmp.getTimeRecord().length)) {
                    totalTime += casEmp.getTimeRecord()[numberOfRecords];
                    numberOfRecords++;
                }
                return totalTime > 10.0;
            }
        });
        for (CasualEmployee casEmp : casEmps){
            System.out.println(casEmp);
        }
        db.close();
        
        // Query for CasualEmployee objects (contains 2.5 & 4.0) - SODA
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE (CONTAINS 2.5 & 4.0) - SODA");
        Query query = db.query();
        query.constrain(CasualEmployee.class);
        Query valuequery = query.descend("_timeRecord");
        valuequery.constrain(2.5);
        valuequery.constrain(4.0);
        results = query.execute();
        listObjectSet(results);
        db.close();
        
        //Update an array - QBE
        Db4o.configure().objectClass(CasualEmployee.class).cascadeOnUpdate(false);
        db = Db4o.openFile(DBFILE);
        casEmpExample = new CasualEmployee("Tim", null, null, 0, null);
        results = db.get(casEmpExample);
        ce = (CasualEmployee)results.next();
        ce.addTimeRecord(6.0);
        ce.getTimeRecord()[0] = 7.0;
        db.set(ce);
        db.close();
        
        db = Db4o.openFile(DBFILE);
        System.out.println("CASUALEMPLOYEE WITH UPDATED ARRAY - QBE");
        casEmpExample = new CasualEmployee("Tim", null, null, 0, null);
        results = db.get(casEmpExample);
        ce = (CasualEmployee)results.next();
        for (int i = 0; i < ce.getTimeRecord().length; i++) {
            System.out.print(ce.getTimeRecord()[i] + " ");
        }
        System.out.println();
        db.close();
        
        System.out.println("\nEND ARRAYS");
    }
    
    private static void collections(){
        ObjectContainer db;
        ObjectSet results;
        Project projectExample;
        Employee emp;
        List<Project> projects;
        
        System.out.println("\nCOLLECTIONS");
        
        resetDatabase();
        
        // Create and store test objects
        Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
        Employee e2 = new Employee("Anne", "5432", "anne", 102, "8/5/1980");
        CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 103, "7/6/1986");
        CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 104, "11/3/1984");
        Project p1 = new Project("Finance System", "P01");
        Project p2 = new Project("Web Site", "P02");
        
        p1.assignEmployee(e1);
        p1.assignEmployee(c1);
        p2.assignEmployee(e2);
        p2.assignEmployee(c2);
        
        db = Db4o.openFile(DBFILE);
        db.set(p1);
        db.set(p2);
        db.close();
        
        // Query for Project by Employee name - QBE
        db = Db4o.openFile(DBFILE);
        System.out.println("PROJECT BY EMPLOYEE NAME - QBE");
        emp = new Employee("Anne", null, null, 0, null);
        List<Employee> empList = new ArrayList<Employee>();
        empList.add(emp);
        projectExample = new Project(null,null,empList);
        results = db.get(projectExample);
        listObjectSet(results);
        db.close();
        
        // Query for Project by Employee name - Native Query
        db = Db4o.openFile(DBFILE);
        System.out.println("PROJECT BY EMPLOYEE NAME - NQ");
        projects = db.query(new Predicate<Project>() {
            public boolean match(Project proj) {
                //               {   // extra bracket? check
                for(Employee empl : proj.employees()) {
                    if (empl.getName().equals("Michael")) {
                        return true;
                    }
                }
                return false;
                //               }  // extra bracket? check
            }
        });
        for(Project project : projects) {
            System.out.println(project);
        }
        db.close();
        
        // Query for Project by Employee name - SODA
        db = Db4o.openFile(DBFILE);
        System.out.println("PROJECT BY EMPLOYEE NAME - SODA");
        Query query = db.query();
        query.constrain(Project.class);
        Query empQuery = query.descend("_employees");
        empQuery.constrain(Employee.class);
        Query nameQuery = empQuery.descend("_name");
        nameQuery.constrain("Michael");
        results = query.execute();
        
        listObjectSet(results);
        db.close();
        
        // Update Project
        Db4o.configure().objectClass(Project.class).cascadeOnUpdate(true);
        db = Db4o.openFile(DBFILE);
        projectExample = new Project(null, "P02");
        results = db.get(projectExample);
        while (results.hasNext()) {
            Project proj = (Project)results.next();
            for(Employee empl : proj.employees()) {
                empl.setEmail(empl.getEmail() + ".web");
            }
            db.set(proj);
        }
        db.close();
        db = Db4o.openFile(DBFILE);
        System.out.println("PROJECT (UPDATED EMPLOYEES) - QBE");
        projectExample = new Project(null, "P02");
        results = db.get(projectExample);
        while (results.hasNext()) {
            Project proj = (Project)results.next();
            System.out.println(proj);
            for(Employee empl : proj.employees()) {
                System.out.println(empl + ": " + empl.getEmail());
            }
        }
        db.close();
        
        // Delete a Project
        //Db4o.configure().objectClass(Project.class).cascadeOnDelete(false);
        db = Db4o.openFile(DBFILE);
        projectExample = new Project(null, "P02");
        results = db.get(projectExample);
        while (results.hasNext()) {
            Project proj = (Project)results.next();
            db.delete(proj.employees());
            db.delete(proj);
        }
        db.close();
        
        db = Db4o.openFile(DBFILE);
        System.out.println("ARRAYLISTS AFTER DELETION OF PROJECT");
        results = db.get(new ArrayList<Employee>());
        while (results.hasNext()) {
            ArrayList<Employee> list = (ArrayList<Employee>)results.next();
            System.out.println(list);
        }
        db.close();
        
        System.out.println("\nEND COLLECTIONS");
        
    }
    
    private static void collectionsActivation() {
        ObjectContainer db;
        ObjectSet results;
        
        System.out.println("\nCOLLECTIONS ACTIVATION");
        System.out.println("no output - set breakpoint and view locals");
        
        resetDatabase();
        
        // Create and store test objects
        Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
        Employee e2 = new Employee("Anne", "5432", "anne", 102, "8/5/1980");
        CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 103, "7/6/1986");
        CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 104, "11/3/1984");
        // Store objects
        db = Db4o.openFile(DBFILE);
        
        Project p1 = new Project("Finance System", "P01");
        Project p2 = new Project("Web Site", "PO2");
        Project p3 = new Project("CRM System", "P03");
        
        p1.assignEmployee(e1);
        p1.assignEmployee(e2);
        p1.assignEmployee(c1);
        p2.assignEmployee(c1);
        p2.assignEmployee(c2);
        p3.assignEmployee(e1);
        p3.assignEmployee(c2);
        
        db.set(e1);
        db.set(e2);
        db.set(c1);
        db.set(c2);
        db.set(p1);
        db.set(p2);
        db.set(p3);
        db.close();
        
        // Query for Employee objects - QBE
        Db4o.configure().activationDepth(1);
        db = Db4o.openFile(DBFILE);
        Employee employeeExample = new Employee("Michael", null, null, 0, null);
        results = db.get(employeeExample);
        Employee emp = (Employee)results.next();
        
        // Set breakpoint and view emp in Locals
        db.activate(emp.projects(), 1);
        // Set breakpoint and view emp again in Locals
        db.close();
        
        System.out.println("\nEND COLLECTIONS ACTIVATION");
    }
    
    private static void deepGraph() {
        ObjectContainer db;
        ObjectSet results;
        
        System.out.println("\nDEEP GRAPH");
        
        resetDatabase();
        
        // Create and store test objects
        Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/05/1975");
        Employee e2 = new Employee("Anne", "5432", "anne", 102, "08/05/1980");
        Employee e3 = new Employee("Jane", "5753", "jane", 103, "10/06/1985");
        Employee e4 = new Employee("Franz", "8765", "franz", 104, "06/05/1980");
        CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 105, "07/06/1986");
        CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 106, "11/03/1984");
        CasualEmployee c3 = new CasualEmployee("Jan", "4455", "jan", 107, "07/09/1976");
        CasualEmployee c4 = new CasualEmployee("Ellen", "3344", "ellen", 108, "12/03/1982");
        Manager m1= new Manager("Sue", "1111", "sue", 109,"01/01/1981");
        
        m1.addEmployee(e1);
        m1.addEmployee(e2);
        m1.addEmployee(e3);
        m1.addEmployee(e4);
        m1.addEmployee(c1);
        m1.addEmployee(c2);
        m1.addEmployee(c3);
        m1.addEmployee(c4);
        
        db = Db4o.openFile(DBFILE);
        db.set(m1);
        db.close();
        
        // Query for Manager - edit to select activation option
        int activationDepth = 5;   // vary this number to test
        Db4o.configure().activationDepth(activationDepth);
        //Db4o.configure().objectClass(Manager.class).minimumActivationDepth(10);
        //Db4o.configure().objectClass(LinkedList.class).cascadeOnActivate(true);
        db = Db4o.openFile(DBFILE);
        System.out.println("MANAGER AND EMPLOYEES - GLOBAL ACTIVATION DEPTH = " + activationDepth);
        Manager managerExample = new Manager("Sue", null, null, 0, null);
        results = db.get(managerExample);
        Manager man = (Manager)results.next();
        System.out.println(man);
        LinkedList list = man.employees();
        while(list!=null){
            System.out.println(list.getItem());
            //db.activate(list.getNext(),2);
            list = list.getNext();
        }
        db.close();
        
        Db4o.configure().activationDepth(5);   // reset to default
        
        System.out.println("\nEND DEEP GRAPH");
    }
    
    private static void composite() {
        ObjectContainer db;
        ObjectSet results;
        
        System.out.println("\nCOMPOSITE");
        
        resetDatabase();
        
        // Employees and Projects
        Manager m1 = new Manager("Sue", "9876", "sue", 101, "3/8/1982");
        Manager m2 = new Manager("Erich", "6543", "erich", 102, "10/1/1963");
        Employee e1 = new Employee("Michael", "1234", "michael", 103, "10/5/1975");
        Employee e2 = new Employee("Anne", "5432", "anne", 104, "8/5/1980");
        Employee e3 = new Employee("Franz", "3456", "franz", 105, "11/8/1985");
        CasualEmployee c1 = new CasualEmployee("Nicole", "4321", "nicole", 106,"12/12/1987");
        CasualEmployee c2 = new CasualEmployee("Dan", "3690", "dan", 107, "1/1/1986");
        m1.addEmployee(m2);
        m1.addEmployee(e3);
        m1.addEmployee(c1);
        m2.addEmployee(e1);
        m2.addEmployee(e2);
        m2.addEmployee(c2);
        Department d1 = new Department("Software", m1);
        Project p1 = new Project("Finance System", "C01");
        Project p2 = new Project("Web Site", "CO2");
        p1.assignEmployee(m2);
        p1.assignEmployee(e1);
        p1.assignEmployee(e2);
        p1.assignEmployee(c1);
        p2.assignEmployee(e1);
        p2.assignEmployee(c1);
        
        
        db = Db4o.openFile(DBFILE);
        db.set(m1);
        db.set(d1);
        db.close();
        
        //Db4o.configure().activationDepth(5);
        
        db = Db4o.openFile(DBFILE);
        System.out.println("MANAGERS AND EMPLOYEES - QBE");
        Manager managerExample = new Manager(null, null, null, 0, null);
        results = db.get(managerExample);
        while (results.hasNext()) {
            Manager man = (Manager)results.next();
            System.out.println("MANAGER: " + man);
            LinkedList list = man.employees();
            while (list != null) {
                System.out.println(list.getItem());
                db.activate(list.getNext(),2);
                list = list.getNext();
            }
        }
        db.close();
        
        System.out.println("\nEND COMPOSITE");
    }
    
    private static void listObjectSet(ObjectSet results) {
        while (results.hasNext()) {
            Object result = results.next();
            System.out.println(result);
        }
    }
}
