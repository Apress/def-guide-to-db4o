
package com.db4o.dg2db4o.chapter11;


public interface ServerConfiguration {
    	public String HOST = "localhost";
	public String REMOTEFILE = "C:/remote.yap"; // gets changes
	public String LOCALFILE = "C:/local.yap"; // produces changes
	public int PORT = 4488;
	public String USER = "myName";
	public String PASS = "myPwd";

}
