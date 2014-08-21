package com.youbetme.androidsample.tasks;

import android.os.AsyncTask;

import com.youbetme.androidsample.proxies.Proxy;

public class GetCustomerFromSessionTask extends AsyncTask<Void, Integer, String> {
	
	private String token;
	
	public GetCustomerFromSessionTask(String token){
		this.token = token;
	}
	
    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }

	@Override
	protected String doInBackground(Void... params) {
        Proxy proxy = new Proxy();
        return proxy.GetCustomerFromSession(this.token);
	}
}
