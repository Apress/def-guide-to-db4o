

package com.db4o.dg2db4o.chapter7;

public class LinkedList {
    private Object _item;
    private LinkedList _next; 
    
   public LinkedList(){
       _item = null;
       _next = null;
   }
    
    public LinkedList(Object item) {
        _item = item;
        _next = null;
    }

    public Object getItem() {
        return _item;
    }

    public void setItem(Object item) {
        _item = item;
    }

    public LinkedList getNext() {
        return _next;
    }

    public void setNext(LinkedList next) {
        _next = next;
    }
    
    public void append(LinkedList node){
        if (_next == null)
                {
                    _next = node;
                }
            else
                {
                    _next.append(node);
                }
    }
}
