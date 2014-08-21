package com.youbetme.androidsample.tasks;

import android.os.AsyncTask;

public abstract class SuperTask extends AsyncTask<String, Integer, String[]> {
	
	protected String token;
	
	protected SuperTask(String token)
	{
		this.token = token;
	}
}
