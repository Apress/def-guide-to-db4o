/*
 * Fruit.java
 *
 * Created on 08 February 2006, 14:40
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package com.db4o.dg2db4o.chapter12;

import com.db4o.ext.ObjectCallbacks;
import com.db4o.ObjectContainer;
import java.util.List;
import java.util.ArrayList;

public class Fruit {
    private String _name;
    private int _amount;
    private List<String> amountHistory;
    
    public Fruit(String name, int amount){
        _name = name;
        _amount = amount;
        amountHistory = new ArrayList<String>();
    }
    
    public boolean objectCanUpdate(ObjectContainer container){
        amountHistory.add(
                new Long(System.currentTimeMillis()).toString() 
                + ", " + _amount);
        System.out.println("I was updated!");
        return false; // Try this with true and false to see the difference
    }
    
    public String toString(){
        String ret="Name=" + _name + "\nAmount=" + _amount;
        for (String s :  amountHistory) {
            ret += "\n Modified at:" + s;
        }
        return ret;
    }
    
    public String getName() {
        return _name;
    }
    
    public void setName(String name) {
        this._name = name;
    }
    
    public int getAmount() {
        return _amount;
    }
    
    public void setAmount(int amount) {
        this._amount = amount;
    }
}
