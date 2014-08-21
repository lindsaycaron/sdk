package com.youbetme.androidsample.tasks;

import android.os.AsyncTask;

import com.youbetme.androidsample.proxies.Proxy;

public class SetDeviceTask extends AsyncTask<Void, Integer, String[]> {
	
	private String registrationId;
	private String customerId;
	
	public SetDeviceTask(String registrationId, String customerId)
	{
		this.registrationId = registrationId;
		this.customerId = customerId;
	}
	
    protected String[] doInBackground(Void...request) {
    	 int count = request.length;
         String[] response = new String[count];
         Proxy proxy = new Proxy();
         for (int i = 0; i < count; i++) {
        	response[i] = proxy.SetDevice(this.registrationId, this.customerId);
         }
         return response;
    }

    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }
}
