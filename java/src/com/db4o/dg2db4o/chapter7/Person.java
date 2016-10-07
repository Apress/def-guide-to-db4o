
package com.db4o.dg2db4o.chapter7;

public interface Person {
    String getName();
    void setName(String name);
    String getPhoneNumber();
    void setPhoneNumber(String phoneNumber);
    String getEmail();
    void setEmail(String email);
    void sendMail(String fromAddress, String subject, String content);
}
