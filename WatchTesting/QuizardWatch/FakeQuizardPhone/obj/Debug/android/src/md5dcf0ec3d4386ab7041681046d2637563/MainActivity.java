package md5dcf0ec3d4386ab7041681046d2637563;


public class MainActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.wearable.DataApi.DataListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onDataChanged:(Lcom/google/android/gms/wearable/DataEventBuffer;)V:GetOnDataChanged_Lcom_google_android_gms_wearable_DataEventBuffer_Handler:Android.Gms.Wearable.IDataApiDataListenerInvoker, Xamarin.GooglePlayServices.Wearable\n" +
			"";
		mono.android.Runtime.register ("QuizardWatch.MainActivity, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainActivity.class, __md_methods);
	}


	public MainActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainActivity.class)
			mono.android.TypeManager.Activate ("QuizardWatch.MainActivity, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onDataChanged (com.google.android.gms.wearable.DataEventBuffer p0)
	{
		n_onDataChanged (p0);
	}

	private native void n_onDataChanged (com.google.android.gms.wearable.DataEventBuffer p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
