package md5dcf0ec3d4386ab7041681046d2637563;


public class MainActivity_MessageReciever
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("QuizardWatch.MainActivity+MessageReciever, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainActivity_MessageReciever.class, __md_methods);
	}


	public MainActivity_MessageReciever () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainActivity_MessageReciever.class)
			mono.android.TypeManager.Activate ("QuizardWatch.MainActivity+MessageReciever, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public MainActivity_MessageReciever (md5dcf0ec3d4386ab7041681046d2637563.MainActivity p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainActivity_MessageReciever.class)
			mono.android.TypeManager.Activate ("QuizardWatch.MainActivity+MessageReciever, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "QuizardWatch.MainActivity, FakeQuizardPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
