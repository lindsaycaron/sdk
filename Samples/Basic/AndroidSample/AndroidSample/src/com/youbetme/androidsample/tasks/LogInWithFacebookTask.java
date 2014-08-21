package com.youbetme.androidsample.tasks;

import com.youbetme.androidsample.proxies.Proxy;

import android.os.AsyncTask;

public class LogInWithFacebookTask extends AsyncTask<String, Integer, String[]> {
	
    protected String[] doInBackground(String...request) {
    	 int count = request.length;
         String[] response = new String[count];
         Proxy proxy = new Proxy();
         for (int i = 0; i < count; i++) {
        	response[i] = proxy.LogInWithFacebook(request[i]);
         }
         return response;
    }

    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }
}

