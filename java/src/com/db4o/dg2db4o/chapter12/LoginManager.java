/*
 * ServerLogin.java
 *
 * Created on 08 February 2006, 23:45
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package com.db4o.dg2db4o.chapter12;

import com.db4o.ObjectContainer;
import com.db4o.Db4o;
import java.io.IOException;

public class LoginManager {
    String _host;
    int _port;
    static final int MAXIMUM_USERS = 4;
    
    public LoginManager(String host, int port) {
        _host = host;
        _port = port;
    }
    
    public ObjectContainer login(String username, String password){
        
        ObjectContainer objectContainer;
        try {
            objectContainer = Db4o.openClient(_host, _port, username, password);
        } catch (IOException e) {
            return null;
        }
        
        boolean allowedToLogin = false;
        
        for (int i = 0; i < MAXIMUM_USERS; i++) {
            String semaphore = "login_limit_" + (i+1);
            if(objectContainer.ext().setSemaphore(semaphore, 0)){
                allowedToLogin = true;
                System.out.println("Logged in as " + username);
                System.out.println("Acquired semaphore " + semaphore);
                break;
            }
        }
        
        if(! allowedToLogin){
            System.out.println("Login not allowed for " + username + ": max clients exceeded");
            objectContainer.close();
            return null;
        }
        
        return objectContainer;
    }

}
