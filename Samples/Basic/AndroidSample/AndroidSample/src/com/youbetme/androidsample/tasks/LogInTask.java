package com.youbetme.androidsample.tasks;

import com.youbetme.androidsample.dataobjects.Credentials;
import com.youbetme.androidsample.proxies.Proxy;

import android.os.AsyncTask;

public class LogInTask extends AsyncTask<Credentials, Integer, String[]> {
		
    protected String[] doInBackground(Credentials...request) {
    	 int count = request.length;
         String[] response = new String[count];
         Proxy proxy = new Proxy();
         for (int i = 0; i < count; i++) {
        	response[i] = proxy.LogIn(request[i].getUsernameOrEmail(), request[i].getPassword());
         }
         return response;
    }

    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }
}
