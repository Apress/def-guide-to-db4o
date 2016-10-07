
package com.db4o.dg2db4o.chapter6;

import java.util.*;
import java.io.File;
import com.db4o.*;
import com.db4o.inside.query.*;
import com.db4o.query.*;

public class Main {
    private static final String DBFILE = "C:/queries.yap";
    
    public static void main(String[] args){
        storeData();
        qbe();
        nativeQueries();
        soda();
    }
    
    private static void storeData(){
        new File(DBFILE).delete();
        ObjectContainer db = Db4o.openFile(DBFILE);
        db.set(new Person("Gandhi", 79));
        db.set(new Person("Lincoln", 56));
        db.set(new Person("Teresa", 86));
        db.set(new Person("Mandela", 86));
        db.set(new Person("John", 42));
        db.set(new Person("Ben", 82));
        db.close();
    }
    
    private static void qbe(){
        ObjectSet res;
        Person template;
        
        System.out.println("\nQBE EXAMPLES");
        
        ObjectContainer db = Db4o.openFile(DBFILE);
        
        // Get all Person objects
        System.out.println("ALL PERSON OBJECTS");
        res = db.get(new Person());
        listResult(res);
        
        // Get by name
        System.out.println("FIND BY NAME");
        template = new Person();
        template.setName("Ben");
        res = db.get(template);
        listResult(res);
        
        // Get by name and age
        System.out.println("FIND BY NAME AND AGE");
        template = new Person();
        template.setName("Ben");
        template.setAge(42);
        res = db.get(template);
        listResult(res);
        
        // Get by age (name is null)
        System.out.println("FIND BY AGE");
        template = new Person(null,82);
        res = db.get(template);
        listResult(res);
        
        // Get all Person objects using class name
        System.out.println("ALL PERSON OBJECTS (CLASS NAME)");
        res = db.get(Person.class);
        listResult(res);
        
        // Get all objects using null
        System.out.println("ALL OBJECTS");
        res = db.get(null);
        listResult(res);
        
        db.close();
        System.out.println("END OF QBE EXAMPLES");
        
    }
    
    private static void nativeQueries(){
        List<Person> persons;
        
        System.out.println("\nNQ EXAMPLES");
        
        ObjectContainer db = Db4o.openFile(DBFILE);
        
        // Add query execution listener to check optimization
        ((YapStream)db).getNativeQueryHandler().addListener(new Db4oQueryExecutionListener() {
            public void notifyQueryExecuted(NQOptimizationInfo info) {
                System.out.println(info.toString());
            }
        });
        
        // Add query execution listener to check optimization - pre 5.2
//            ((YapStream)db).getNativeQueryHandler().addListener(new Db4oQueryExecutionListener() {
//                public void notifyQueryExecuted(Predicate filter, String msg) {
//                    System.out.println(filter.extentType().getName());
//                    System.out.println(msg);
//                }
//            });
        
        // Simple query
        System.out.println("SIMPLE NQ - AGE > 60");
        persons = db.query(new Predicate<Person>() {
            public boolean match(Person person) {
                return person.getAge() > 60;
            }
        });
        for (Person person : persons)
            System.out.println(person);
        
        // Range query
        System.out.println("RANGE NQ - AGE BETWEEN 60 AND 80");
        persons = db.query(new Predicate<Person>() {
            public boolean match(Person person) {
                return person.getAge() < 60 || person.getAge() > 80;
            }
        });
        for (Person person : persons)
            System.out.println(person);
        
        // Compound query
        System.out.println("COMPOUND NQ - AGE >80 AND NAME='MANDELA'");
        persons = db.query(new Predicate<Person>() {
            public boolean match(Person person) {
                return person.getAge() > 80 && person.getName().equals("Mandela");
                //return person.getAge() > 80 && person.getName()== "Mandela";
            }
        });
        for (Person person : persons)
            System.out.println(person);
        
        // Sorted query using 5.2 sorted NQ
        System.out.println("SORTED NQ - ALL PERSONS");
        // Set up Comparator
        Comparator<Person> personCmp=
                new Comparator<Person>() {
            public int compare(Person o1, Person o2) {
                return o1.getName()
                .compareTo(o2.getName());
            }
        };
        // Query with Comparator
        List<Person> result = db.query(new Predicate<Person>() {
            public boolean match(Person person) {
                return true;
            }
        },personCmp);
        for (Person person : result)
            System.out.println(person);
        
        // Sorted query pre 5.2 using array copy
        System.out.println("SORTED NQ - USING ARRAY COPY");
        persons = db.query(new Predicate<Person>() {
            public boolean match(Person person) {
                return true;
            }
        });
        Object p[] = persons.toArray();
        System.out.println("after sorting");
        Arrays.sort(p);
        for(int i=0; i<p.length; i++) {
            System.out.println(p[i]);
        }
        
        db.close();
        System.out.println("END OF NQ EXAMPLES");
    }
    
    private static void soda(){
        ObjectSet res;
        Query query;
        
        System.out.println("\nSODA EXAMPLES");
        
        ObjectContainer db = Db4o.openFile(DBFILE);
        
        // Get all Persons
        System.out.println("SODA - ALL PERSONS");
        query = db.query();
        query.constrain(Person.class);
        res = query.execute();
        listResult(res);
        
        // Get by name
        System.out.println("SODA - GET BY NAME");
        query = db.query();
        query.constrain(Person.class);
        query.descend("_name").constrain("Lincoln"); // search a name
        //query.descend("_placeOfBirth").constrain("Hardin County"); // uncomment to try query by invalid field
        res = query.execute();
        listResult(res);
        
        // "Not" query
        System.out.println("SODA - AGE NOT 56");
        query = db.query();
        query.constrain(Person.class);
        query.descend("_age").constrain(56).not(); // not 56
        res = query.execute();
        listResult(res);
        
        // Compound query
        System.out.println("SODA - AGE= 86 AND NAME = 'MANDELA'");
        query = db.query();
        query.constrain(Person.class);
        Constraint firstConstr = query.descend("_age").constrain(86); // first constraint
        query.descend("_name").constrain("Mandela").and(firstConstr); // second using and
        res = query.execute();
        listResult(res);
        
        // Compound query (alternative version)
        System.out.println("SODA - AGE= 86 AND NAME = 'MANDELA' (ALT)");
        query = db.query();
        query.constrain(Person.class);
        query.descend("_age").constrain(86); // first constraint
        query.descend("_name").constrain("Mandela");
        res = query.execute();
        listResult(res);
        
        // "Or" query
        System.out.println("SODA - AGE= 86 OR NAME = 'LINCOLN'");
        query = db.query();
        query.constrain(Person.class);
        firstConstr = query.descend("_age").constrain(86); // first constraint
        query.descend("_name").constrain("Lincoln").or(firstConstr); // second using and
        res = query.execute();
        listResult(res);
        
        // Range query
        System.out.println("SODA - AGE BETWEEN 60 AND 80");
        query = db.query();
        query.constrain(Person.class);
        firstConstr = query.descend("_age").constrain(60).greater(); // first constraint
        query.descend("_age").constrain(80).smaller().and(firstConstr); // second using and
        res = query.execute();
        listResult(res);
        
        // "Greater" query
        System.out.println("SODA - AGE > 80");
        query = db.query();
        query.constrain(Person.class);
        query.descend("_age").constrain(80).greater();
        res = query.execute();
        listResult(res);
        
        // "Like" query
        System.out.println("SODA - LIKE 'Ma'");
        query=db.query();
        query.constrain(Person.class);
        query.descend("_name").constrain("Ma").like(); // also works with "ma"
        res = query.execute();
        listResult(res);
        
        // Sorted query
        System.out.println("SORTED SODA - ALL PERSONS");
        query = db.query();
        query.constrain(Person.class);
        query.descend("_name").orderAscending(); // the list should start with the lowest
        res = query.execute();
        listResult(res);
        
        // Null/zero value query
        Person p = new Person();
        p.setName("Fred");
        db.set(p);
        db.commit();
        System.out.println("SODA - NULL/ZERO VALUE");
        query=db.query();
        query.constrain(Person.class);
        query.descend("_age").constrain(0); // field has been set
        res=query.execute();
        listResult(res);
        
        db.close();
        System.out.println("END OF SODA EXAMPLES");
        
    }
    
    private static void listResult(ObjectSet res){
        while(res.hasNext())
            System.out.println(res.next());
    }
    
}
