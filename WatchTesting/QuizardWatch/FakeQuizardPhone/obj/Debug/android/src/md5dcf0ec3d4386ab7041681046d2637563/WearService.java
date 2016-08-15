package md5dcf0ec3d4386ab7041681046d2637563;


public class WearService
	extends com.google.android.gms.wearable.WearableListenerService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:()V:GetOnCreateHandler\n" +
			"n_onDataChanged:(Lcom/google/android/gms/wearable/DataEventBuffer;)V:GetOnDataChanged_Lcom_google_android_gms_wearable_DataEventBuffer_Handler\n" +
			"";
		mono.android.Runtime.register ("QuizardWatch.WearService, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", WearService.class, __md_methods);
	}


	public WearService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WearService.class)
			mono.android.TypeManager.Activate ("QuizardWatch.WearService, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate ()
	{
		n_onCreate ();
	}

	private native void n_onCreate ();


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
