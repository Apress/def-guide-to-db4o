
package com.db4o.dg2db4o.chapter7;


public class ZipCode {
    private String _state;
    private String _code;
    private String _extension;
    
    public ZipCode(String state, String code, String extension) {
        _state = state;
        _code= code;
        _extension = extension;
    }
    
    public String getState() {
        return _state;
    }
    
    public void setState(String state) {
        this._state = state;
    }
    
    public String getCode() {
        return _code;
    }
    
    public void setCode(String code) {
        this._code = code;
    }
    
    public String getExtension() {
        return _extension;
    }
    
    public void setExtension(String extension) {
        this._extension = extension;
    }
    
    public String toString() {
        return _state + _code + "-" + _extension + " (ZipCode)";
    }
    
}
