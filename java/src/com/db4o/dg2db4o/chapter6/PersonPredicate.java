/*
 * PersonPredicate.java
 *
 * Created on 06 February 2006, 12:06
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package com.db4o.dg2db4o.chapter6;

import com.db4o.query.*;

/**
 *
 * @author jpa1
 */
public class PersonPredicate extends Predicate{

    public boolean match(Object o) {
        return true;
    }

}
