using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using com.db4o;
using com.db4o.query;

namespace com.db4o.dg2db4o.chapter7
{
    class ObjectExamples
    {
        private static string DBFILE = "C:/objects.yap";

        public static void Main(string[] args)
        {
            SimpleStructuredObjects();
            Hierarchy();
            Inheritance();
            Arrays();
            Collections();
            CollectionsActivation();
            DeepGraph();
            Composite();
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }


        private static void ResetDatabase()
        {
            File.Delete(DBFILE);
        }

        private static void SimpleStructuredObjects()
        {

            ObjectContainer db;
            IList<Customer> customers;
            IList<Address> addresses;
            Customer customerExample;
            Address addressExample;
            ObjectSet results;
            Customer cu;

            ResetDatabase();

            Console.WriteLine("\nSIMPLE STRUCTURED OBJECTS");

            // Create test objects
            Address a1 = new Address("1 First Street", "San Jose", "USA");
            Customer cu1 = new Customer("Gary", "408 123 4567", "gary@someisp.com", a1);

            // Store Customer or Address
            db = Db4o.OpenFile(DBFILE);
            db.Set(cu1);
            //db.Set(a1);
            db.Close();


            // Retrieve and display Customer - QBE
            Console.WriteLine("FIND CUSTOMER: QBE");
            db = Db4o.OpenFile(DBFILE);
            customerExample = new Customer("Gary", null, null, new Address(null, null, null));
            results = db.Get(customerExample);

            while (results.HasNext())
            {
                Customer customer = (Customer)results.Next();
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
            }
            db.Close();

            // Retrieve and display Customer - Native Query
            Console.WriteLine("FIND CUSTOMER: NQ");
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Name.Equals("Gary");
            });
            foreach (Customer customer in customers)
            {
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
            }
            db.Close();

            // Retrieve and display Address - QBE
            Console.WriteLine("FIND ADDRESS: QBE");
            db = Db4o.OpenFile(DBFILE);
            addressExample = new Address("1 First Street", null, null);
            results = db.Get(addressExample);
            while (results.HasNext())
            {
                Address address = (Address)results.Next();
                Console.WriteLine(address);
            }
            db.Close();

            // Retrieve and display Address - Native Query
            Console.WriteLine("FIND ADDRESS: NQ");
            db = Db4o.OpenFile(DBFILE);
            addresses = db.Query<Address>(delegate(Address add)
            {
                return add.Street.Equals("1 First Street");
            });
            foreach (Address address in addresses)
            {
                Console.WriteLine(address);
            }
            db.Close();

            // Retrieve and display Customer by Address - Native Query
            Console.WriteLine("FIND CUSTOMER BY ADDRESS: NQ");
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Address.Street.Equals("1 First Street");
            });
            foreach (Customer customer in customers)
            {
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
            }
            db.Close();

            // Retrieve and display Customer by Address - SODA
            Console.WriteLine("FIND CUSTOMER BY ADDRESS: SODA");
            db = Db4o.OpenFile(DBFILE);
            Query query = db.Query();
            query.Constrain(typeof(Customer));
            query.Descend("_address").Descend("_street").Constrain("1 First Street");
            results = query.Execute();
            while (results.HasNext())
            {
                Customer customer = (Customer)results.Next();
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
            }
            db.Close();


            // Retrieve Customer and update Address
            Db4o.Configure().ObjectClass(typeof(Customer)).CascadeOnUpdate(true);
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Name.Equals("Gary");
            });
            cu = customers[0];  // get first returned Customer - should only be one
            cu.Address.Street = "2 Second Street";
            db.Set(cu);
            db.Close();
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Name.Equals("Gary");
            });
            Console.WriteLine("CUSTOMER AFTER UPDATE");
            foreach (Customer customer in customers)
            {
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
            }
            db.Close();

            // Test activation
            Db4o.Configure().ActivationDepth(1);
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Name.Equals("Gary");
            });
            cu = customers[0];  // get first returned Customer - should only be one
            Console.WriteLine("ACTIVATION - no output, set breakpoints");
            db.Activate(cu.Address, 1);
            db.Close();
            Db4o.Configure().ActivationDepth(5);

            // Delete Customer and test cascading
            Db4o.Configure().ObjectClass(typeof(Customer)).CascadeOnDelete(true);
            db = Db4o.OpenFile(DBFILE);
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Name.Equals("Gary");
            });
            cu = customers[0];  // get first returned Customer - should only be one
            //db.Delete(cu);
            db.Close();
            db = Db4o.OpenFile(DBFILE);
            addresses = db.Query<Address>(delegate(Address add)
            {
                return add.Street.Equals("2 Second Street");
            });
            Console.WriteLine("ADDRESS AFTER CUSTOMER DELETED");
            foreach (Address address in addresses)
            {
                Console.WriteLine(address);
            }
            db.Close();

            Console.WriteLine("\nEND SIMPLE STRUCTURED OBJECTS");
        }

        private static void Hierarchy()
        {
            ObjectContainer db;
            IList<Customer> customers;
            IList<Address> addresses;
            Customer customerExample;
            Address addressExample;
            ZipCode zipExample;
            ObjectSet results;
            Customer cu;

            ResetDatabase();

            Console.WriteLine("\nOBJECT HIERARCHY");

            // Create test objects
            ZipCode z1 = new ZipCode("CA", "95200", "1234");
            ZipCode z2 = new ZipCode("CA", "95200", "5678");
            Address a1 = new Address("1 First Street", "San Jose", "USA");
            a1.ZipCode = z1;
            Address a2 = new Address("2 Second Street", "San Jose", "USA");
            a2.ZipCode = z2;
            Customer cu1 = new Customer("Gary", "408 123 4567", "gary@example.net", a1);
            Customer cu2 = new Customer("Mary", "408 101 1001", "mary@example.com", a2);

            // Store Customers
            db = Db4o.OpenFile(DBFILE);
            db.Set(cu1);
            db.Set(cu2);
            db.Close();

            // Retrieve and display Customer by ZipCode - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CUSTOMER BY ZIPCODE: QBE");
            zipExample = new ZipCode(null, null, "5678");
            addressExample = new Address(null, null, null);
            addressExample.ZipCode = zipExample;
            customerExample = new Customer(null, null, null, addressExample);
            results = db.Get(customerExample);

            while (results.HasNext())
            {
                Customer customer = (Customer)results.Next();
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
                Console.WriteLine(customer.Address.ZipCode);
            }
            db.Close();

            // Retrieve and display Customer by ZipCode - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CUSTOMER BY ZIPCODE: NQ");
            customers = db.Query<Customer>(delegate(Customer cust)
            {
                return cust.Address.ZipCode.Extension.Equals("1234");
            });
            foreach (Customer customer in customers)
            {
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
                Console.WriteLine(customer.Address.ZipCode);
            }
            db.Close();

            // Retrieve and display Customer by ZipCode - SODA
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CUSTOMER BY ZIPCODE: SODA");
            Query query = db.Query();
            query.Constrain(typeof(Customer));
            //query.Descend("_address").Descend("_zipCode").Descend("_extension").Constrain("5678");
            Query zipQuery = query.Descend("_address").Descend("_zipCode");
            zipQuery.Descend("_extension").Constrain("5678");
            results = query.Execute();
            while (results.HasNext())
            {
                Customer customer = (Customer)results.Next();
                Console.WriteLine(customer);
                Console.WriteLine(customer.Address);
                Console.WriteLine(customer.Address.ZipCode);
            }
            db.Close();

            Console.WriteLine("\nEND OBJECT HIERARCHY");
        }


        private static void Inheritance()
        {
            ObjectContainer db;
            ObjectSet results;

            ResetDatabase();

            Console.WriteLine("\nINHERITANCE");

            // Create test objects
            Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
            Manager m1 = new Manager("Sue", "9876", "sue", 102, "3/8/1982");
            CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 103, "7/6/1986");
            Customer cu1 = new Customer("Gary", "408 123 4567", "gary@someisp.com", new Address("1 First Street", "San Jose", "USA"));

            // Store objects
            db = Db4o.OpenFile(DBFILE);
            db.Set(e1);
            db.Set(m1);
            db.Set(c1);
            db.Set(cu1);
            db.Close();

            // Query for Employee objects - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("FIND EMPLOYEES: QBE");
            Employee employeeExample = new Employee(null, null, null, 0, null);
            results = db.Get(employeeExample);
            ListObjectSet(results);
            db.Close();

            // Query for Manager objects - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("FIND MANAGERS: QBE");
            Manager managerExample = new Manager(null, null, null, 0, null);
            results = db.Get(managerExample);
            ListObjectSet(results);
            db.Close();

            // Query for Person objects - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("FIND PERSONS (ABSTRACT): QBE");
            results = db.Get(typeof(Person));
            ListObjectSet(results);
            db.Close();

            // Query for Person objects - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("FIND PERSONS (ABSTRACT): NQ");
            IList<Person> persons = db.Query<Person>(delegate(Person pers)
            {
                return true;
            });
            foreach (Person person in persons)
            {
                Console.WriteLine(person);
            }
            db.Close();


            // Query for IPerson objects - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("FIND IPERSONS (INTERFACE): QBE");
            results = db.Get(typeof(IPerson));
            ListObjectSet(results);
            db.Close();

            Console.WriteLine("\nEND INHERITANCE");
        }

        private static void Arrays()
        {
            ObjectContainer db;
            ObjectSet results;
            CasualEmployee casEmpExample;
            IList<CasualEmployee> casEmps;

            ResetDatabase();

            Console.WriteLine("\nARRAYS");

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
            db = Db4o.OpenFile(DBFILE);
            db.Set(c1);
            db.Set(c2);
            db.Close();

            // Query for CasualEmployee objects (contains 4.0) - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 4.0): QBE");
            casEmpExample = new CasualEmployee(null, null, null, 0, null);
            casEmpExample.TimeRecord = new double[] { 4.0 };
            results = db.Get(casEmpExample);
            ListObjectSet(results);
            db.Close();

            // Query for CasualEmployee objects (contains 2.5 & 4.0) - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 2.5 AND 4.0): QBE");
            casEmpExample = new CasualEmployee(null, null, null, 0, null);
            casEmpExample.TimeRecord = new double[] { 2.5, 4.0 };
            results = db.Get(casEmpExample);
            ListObjectSet(results);
            db.Close();

            // Query for CasualEmployee objects (contains 3.5 & 5.0) - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 3.5 AND 5.0): QBE");
            casEmpExample = new CasualEmployee(null, null, null, 0, null);
            casEmpExample.TimeRecord = new double[] { 3.5, 5.0 };
            results = db.Get(casEmpExample);
            ListObjectSet(results);
            db.Close();

            // Try to query for array directly
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("ARRAY (CONTAINS 2.5 AND 4.0): QBE");
            results = db.Get(new double[] { 2.5, 4.0 });
            ListObjectSet(results);
            db.Close();

            // Query for CasualEmployee objects (contains 4.0) - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 4.0): NQ");
            casEmps = db.Query<CasualEmployee>(delegate(CasualEmployee casEmp)
            {
                return Array.IndexOf(casEmp.TimeRecord, 4.0) > -1;
            });
            foreach (CasualEmployee casEmp in casEmps)
            {
                Console.WriteLine(casEmp);
            }
            db.Close();

            // Query for CasualEmployee objects (contains 2.5 & 4.0) - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 2.5 & 4.0): NQ");
            casEmps = db.Query<CasualEmployee>(delegate(CasualEmployee casEmp)
            {
                return Array.IndexOf(casEmp.TimeRecord, 2.5) > -1 && Array.IndexOf(casEmp.TimeRecord, 4.0) > -1;
            });
            foreach (CasualEmployee casEmp in casEmps)
            {
                Console.WriteLine(casEmp);
            }
            db.Close();

            // Query for CasualEmployee objects (Total Time > 10) - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (TOTAL TIME > 10): NQ");
            casEmps = db.Query<CasualEmployee>(delegate(CasualEmployee casEmp)
            {
                double total = 0.0;
                int numberOfRecords = 0;
                while ((casEmp.TimeRecord[numberOfRecords] != 0) && (numberOfRecords < casEmp.TimeRecord.Length))
                {
                    total += casEmp.TimeRecord[numberOfRecords];
                    numberOfRecords++;
                }
                return total > 10.0;
            });
            foreach (CasualEmployee casEmp in casEmps)
            {
                Console.WriteLine(casEmp);
            }
            db.Close();

            // Query for CasualEmployee objects (contains 2.5 & 4.0) - S.O.D.A.
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE (CONTAINS 2.5 & 4.0): SODA");
            Query query = db.Query();
            query.Constrain(typeof(CasualEmployee));
            Query valuequery = query.Descend("_timeRecord");
            valuequery.Constrain(2.5);
            valuequery.Constrain(4.0);
            results = query.Execute();
            ListObjectSet(results);
            db.Close();

            //Update an array - QBE
            Db4o.Configure().ObjectClass(typeof(CasualEmployee)).CascadeOnUpdate(false);
            db = Db4o.OpenFile(DBFILE);
            casEmpExample = new CasualEmployee("Tim", null, null, 0, null);
            results = db.Get(casEmpExample);
            CasualEmployee ce = (CasualEmployee)results.Next();
            ce.addTimeRecord(6.0);
            ce.TimeRecord[0] = 7.0;
            db.Set(ce);
            db.Close();

            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("CASUALEMPLOYEE WITH UPDATED ARRAY: QBE");
            casEmpExample = new CasualEmployee("Tim", null, null, 0, null);
            results = db.Get(casEmpExample);
            ce = (CasualEmployee)results.Next();
            for (int i = 0; i < ce.TimeRecord.GetLength(0); i++)
            {
                Console.Write(ce.TimeRecord[i] + " ");
            }
            Console.WriteLine();
            db.Close();

            Console.WriteLine("\nEND ARRAYS");
        }

        private static void Collections()
        {
            ObjectContainer db;
            ObjectSet results;
            Project projectExample;
            Employee emp;
            IList<Project> projects;

            ResetDatabase();

            Console.WriteLine("\nCOLLECTIONS");

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

            db = Db4o.OpenFile(DBFILE);
            db.Set(p1);
            db.Set(p2);
            db.Close();

            // Query for Project by Employee name - QBE
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("PROJECT BY EMPLOYEE NAME: QBE");
            emp = new Employee("Anne", null, null, 0, null);
            IList empList = new ArrayList();
            //IList<Employee> empList = new List<Employee>();
            empList.Add(emp);
            projectExample = new Project(null, null, empList);
            results = db.Get(projectExample);
            ListObjectSet(results);
            db.Close();



            // Query for Project by Employee name - Native Query
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("PROJECT BY EMPLOYEE NAME: NQ");
            projects = db.Query<Project>(delegate(Project proj)
            {
                foreach (Employee empl in proj.Employees)
                {
                    if (empl.Name.Equals("Michael"))
                    {
                        return true;
                    }
                }
                return false;
            });
            foreach (Project project in projects)
            {
                Console.WriteLine(project);
            }
            db.Close();

            // Query for Project by Employee name - SODA
            db = Db4o.OpenFile(DBFILE);
            Console.WriteLine("PROJECT BY EMPLOYEE NAME: SODA");
            Query query = db.Query();
            query.Constrain(typeof(Project));

            Query empQuery = query.Descend("_employees");
            empQuery.Constrain(typeof(Employee));

            Query nameQuery = empQuery.Descend("_name");
            nameQuery.Constrain("Michael");

            results = query.Execute();
            ListObjectSet(results);
            db.Close();

            // Update Project
            Db4o.Configure().ObjectClass(typeof(Project)).CascadeOnUpdate(true);
            db = Db4o.OpenFile(DBFILE);
            projectExample = new Project(null, "P02");
            results = db.Get(projectExample);
            while (results.HasNext())
            {
                Project proj = (Project)results.Next();
                foreach (Employee empl in proj.Employees)
                {
                    empl.Email = empl.Email + ".web";
                }
                db.Set(proj);
            }
            db.Close();
            db = Db4o.OpenFile(DBFILE);
            projectExample = new Project(null, "P02");
            results = db.Get(projectExample);
            Console.WriteLine("PROJECT (UPDATED EMPLOYEES): QBE");
            while (results.HasNext())
            {
                Project proj = (Project)results.Next();
                Console.WriteLine(proj);
                foreach (Employee empl in proj.Employees)
                {
                    Console.WriteLine(empl + ": " + empl.Email);
                }
            }
            db.Close();

            // Delete a Project
            //Db4o.Configure().ObjectClass(typeof(Project)).CascadeOnDelete(true);
            db = Db4o.OpenFile(DBFILE);
            projectExample = new Project(null, "P02");
            results = db.Get(projectExample);
            while (results.HasNext())
            {
                Project proj = (Project)results.Next();
                db.Delete(proj.Employees);
                db.Delete(proj);
            }
            db.Close();
            db = Db4o.OpenFile(DBFILE);
            results = db.Get(new ArrayList());
            Console.WriteLine("ARRAYLISTS AFTER DELETION OF PROJECT");
            while (results.HasNext())
            {
                ArrayList list = (ArrayList)results.Next();
                Console.WriteLine(list);
            }
            db.Close();

            Console.WriteLine("\nEND COLLECTIONS");
        }


        private static void CollectionsActivation()
        {
            ObjectContainer db;
            ObjectSet results;

            ResetDatabase();

            Console.WriteLine("\nCOLLECTIONS ACTIVATION");
            Console.WriteLine("no output - set breakpoint and view locals");

            // Create and store test objects
            Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
            Employee e2 = new Employee("Anne", "5432", "anne", 102, "8/5/1980");
            CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 103, "7/6/1986");
            CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 104, "11/3/1984");
            // Store objects
            db = Db4o.OpenFile(DBFILE);

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

            db.Set(e1);
            db.Set(e2);
            db.Set(c1);
            db.Set(c2);
            db.Set(p1);
            db.Set(p2);
            db.Set(p3);
            db.Close();

            // Query for Employee objects - QBE
            Db4o.Configure().ActivationDepth(1);
            db = Db4o.OpenFile(DBFILE);
            Employee employeeExample = new Employee("Michael", null, null, 0, null);
            results = db.Get(employeeExample);
            Employee emp = results.Next() as Employee;

            // Set breakpoint and view emp in Locals
            Console.WriteLine("EMPLOYEE QBE");
            db.Activate(emp.Projects, 1);
            // Set breakpoint and view emp again in Locals
            db.Close();

            Console.WriteLine("\nEND COLLECTIONS ACTIVATION");
        }

        private static void DeepGraph()
        {
            ObjectContainer db;
            ObjectSet results;

            ResetDatabase();

            Console.WriteLine("\nDEEP GRAPH");

            // Create and store test objects
            Employee e1 = new Employee("Michael", "1234", "michael", 101, "10/5/1975");
            Employee e2 = new Employee("Anne", "5432", "anne", 102, "8/5/1980");
            Employee e3 = new Employee("Jane", "5753", "jane", 103, "10/6/1985");
            Employee e4 = new Employee("Franz", "8765", "anne", 104, "6/5/1980");
            CasualEmployee c1 = new CasualEmployee("Tim", "5544", "tim", 105, "7/6/1986");
            CasualEmployee c2 = new CasualEmployee("Eva", "4433", "eva", 106, "11/3/1984");
            CasualEmployee c3 = new CasualEmployee("Jan", "4455", "jan", 107, "7/9/1976");
            CasualEmployee c4 = new CasualEmployee("Ellen", "3344", "ellen", 108, "12/3/1982");
            Manager m1 = new Manager("Sue", "1111", "sue", 109, "1/1/1981");

            m1.addEmployee(e1);
            m1.addEmployee(e2);
            m1.addEmployee(e3);
            m1.addEmployee(e4);
            m1.addEmployee(c1);
            m1.addEmployee(c2);
            m1.addEmployee(c3);
            m1.addEmployee(c4);

            db = Db4o.OpenFile(DBFILE);
            db.Set(m1);
            db.Close();

            Db4o.Configure().ActivationDepth(5);
            //Db4o.Configure().ObjectClass(typeof(Manager)).MinimumActivationDepth(10);
            //Db4o.Configure().ObjectClass(typeof(LinkedList)).CascadeOnActivate(true);
            db = Db4o.OpenFile(DBFILE);
            Manager managerExample = new Manager("Sue", null, null, 0, null);
            results = db.Get(managerExample);
            Manager man = (Manager)results.Next();
            Console.WriteLine("EMPLOYEE LIST FOR SPECIFIED MANAGER");
            Console.WriteLine(man);
            LinkedList list = man.Employees;
            while (list != null)
            {
                Console.WriteLine(list.Item);
                //db.Activate(list.Next, 2);
                list = list.Next;
            }
            db.Close();

            Console.WriteLine("\nEND DEEP GRAPH");
        }

        private static void Composite()
        {
            ObjectContainer db;
            ObjectSet results;

            ResetDatabase();

            Console.WriteLine("\nCOMPOSITE");

            // Create and store test objects
            Manager m1 = new Manager("Sue", "9876", "sue", 101, "3/8/1982");
            Manager m2 = new Manager("Erich", "6543", "erich", 102, "10/1/1963");
            Employee e1 = new Employee("Michael", "1234", "michael", 103, "10/5/1975");
            Employee e2 = new Employee("Anne", "5432", "anne", 104, "8/5/1980");
            Employee e3 = new Employee("Franz", "3456", "franz", 105, "11/8/1985");
            CasualEmployee c1 = new CasualEmployee("Nicole", "4321", "nicole", 106, "12/12/1987");
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

            db = Db4o.OpenFile(DBFILE);
            db.Set(d1);
            db.Close();

            Db4o.Configure().ActivationDepth(5);
            //Db4o.Configure().ObjectClass(typeof(LinkedList)).CascadeOnActivate(true);
            db = Db4o.OpenFile(DBFILE);
            Manager managerExample = new Manager(null, null, null, 0, null);
            results = db.Get(managerExample);
            Console.WriteLine("MANAGERS AND THEIR EMPLOYEES");
            while (results.HasNext())
            {
                Manager man = (Manager)results.Next();
                Console.WriteLine("MANAGER: " + man);
                LinkedList list = man.Employees;
                while (list != null)
                {
                    Console.WriteLine(list.Item);
                    db.Activate(list.Next, 2);
                    list = list.Next;
                }
            }
            db.Close();

            Console.WriteLine("\nEND COMPOSITE");
        }

        private static void ListObjectSet(ObjectSet results)
        {
            while (results.HasNext())
            {
                Object result = results.Next();
                Console.WriteLine(result);
            }
        }
    }
}
