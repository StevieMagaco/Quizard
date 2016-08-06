package md578c1a7d2435980346ab2e90830774191;


public class RecyclerAdapter_FlashSetView
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Quizard.RecyclerAdapter+FlashSetView, Quizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RecyclerAdapter_FlashSetView.class, __md_methods);
	}


	public RecyclerAdapter_FlashSetView (android.view.View p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == RecyclerAdapter_FlashSetView.class)
			mono.android.TypeManager.Activate ("Quizard.RecyclerAdapter+FlashSetView, Quizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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
