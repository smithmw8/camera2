using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ShareUIController : MonoBehaviour {

	public GameObject m_contentObj;
	public MaskUIController maskcontroller;
	// Use this for initialization
	void Start () {
 	
	}



	public void Show()
	{
		if (maskcontroller != null) {
			maskcontroller.Show ();
		}
		m_contentObj.transform.Find ("InputField").GetComponent<InputField> ().text = "";
		m_contentObj.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);

	}

	public void ShowMask()
	{

	}


	public void Hide()
	{
	
		m_contentObj.GetComponent<RectTransform> ().DOLocalMove (new Vector3(250,0,0),0.6f);
	}
}
