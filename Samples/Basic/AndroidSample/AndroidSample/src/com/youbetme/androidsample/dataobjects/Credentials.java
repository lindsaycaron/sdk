package com.youbetme.androidsample.dataobjects;

public class Credentials {
	private String usernameOrEmail;
	private String password;
	
	public String getUsernameOrEmail() {
        return usernameOrEmail;
    }
    
    public void setUsernameOrEmail(String value) {
        this.usernameOrEmail = value;
    }
    
    public String getPassword() {
        return password;
    }
    
    public void setPassword(String value) {
        this.password = value;
    }
}
