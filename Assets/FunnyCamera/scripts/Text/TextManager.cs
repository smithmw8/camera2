//----------------------------------------------
// FunnyCamera - realtime camera effects
// Copyright © 2015-2016 52cwalk team
// Contact us (lycwalk@gmail.com)
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class TextManager : MonoBehaviour {

	public Font[] fontARR;
	public Color[] colorARR;
	List<TextItemController> m_textItemControllerList;
	public GameObject instanceTextObj;
	public Transform m_SelectedItemObj;
	public Slider e_ColorSlider;
	public Image e_colorSliderHandleObj;
	public Slider e_AlphaSlider;
	public ComboBox e_comboBox;

	bool isPressed = false;
	bool m_isNeedApply = false;
	public bool isNeedApply
	{
		get 
		{
			return m_isNeedApply;
		}
	}
	Vector3 ScreenSpace = Vector3.zero;
	Vector3 positionOffset = Vector3.zero;
	bool isLongPressd = false;
	bool isMoveItem = false;

	bool isRotatePressed = false;
	Quaternion orgRotation;
	float rotateOffset = 0;
	Vector3 orgScale;
	bool isScalePressed;
	float distanceOffset;

	// Use this for initialization
	void Start () {
		m_textItemControllerList = new List<TextItemController> ();
		isPressed = false;
		isWork = false;
		if (e_comboBox != null) {
			e_comboBox.OnSelectionChanged += onComboBoxSelected;
		}
		if (e_ColorSlider != null) {
			EventTriggerListener.Get(e_ColorSlider.gameObject).onDown += ColorSliderDown;
			EventTriggerListener.Get(e_ColorSlider.gameObject).onUp += ColorSliderUp;
		}
	}

	// Update is called once per frame
	void Update () {
		if (isWork) {
			if (Input.GetMouseButtonDown (0)) {
				if(!AppManager.IsMouseOverUI)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit)) {
						isPressed = true;
						if(hit.collider.tag == "TextItem")
						{
							if(m_SelectedItemObj != hit.collider.transform)
							{
								if(this.m_SelectedItemObj != null)
								{
									this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
								}
							}
							this.m_SelectedItemObj = hit.collider.transform;

							isLongPressd = true;
							StartCoroutine(startClicked(0.3f));
							this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(true);
							isMoveItem = true;

							m_isNeedApply = true;
							ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 

							positionOffset = this.m_SelectedItemObj.transform.position -Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z)) ;

						}
						else if(hit.collider.tag == "TextItemCloseBtn")
						{
							if(this.m_SelectedItemObj != null)
							{
								this.m_SelectedItemObj.GetComponent<TextItemController>().DeleteSelfObj();
								this.m_SelectedItemObj = null;
							}
						}
						else if(hit.collider.tag == "TextItemRotateScaleBtn")
						{
							if(this.m_SelectedItemObj != null)
							{
								isRotatePressed = true;
								Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
								Vector3	ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 
								var dir = movePos - ScreenSpace; 
								rotateOffset = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
								orgRotation =this.m_SelectedItemObj.transform.rotation;

								orgScale = this.m_SelectedItemObj.transform.localScale;
								isScalePressed = true;
								movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
								ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 
								distanceOffset = Vector3.Distance(movePos ,ScreenSpace);

								this.m_SelectedItemObj.GetComponent<TextMeshInput>().StopWork();
							}
						}
						else
						{
							if(this.m_SelectedItemObj != null)
							{
								this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
								this.m_SelectedItemObj = null;
							}
						}
					}
					else
					{
						if(this.m_SelectedItemObj != null)
						{
							this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
							this.m_SelectedItemObj = null;
						}
					}
				}
			}
			
			if (isPressed) {
				#if UNITY_EDITOR
				if(Input.GetMouseButton(0))	
				#elif UNITY_ANDROID||UNITY_IOS||UNITY_TIZEN
					if(Input.GetTouch(0).phase== TouchPhase.Moved && Input.touchCount == 1)
						#endif
				{
					if(isMoveItem)
					{
						Vector3 curScreenSpace =  new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z);  
						Vector3 CurPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + positionOffset;
						this.m_SelectedItemObj.transform.position = new Vector3(CurPosition.x,CurPosition.y,this.m_SelectedItemObj.transform.position.z);  
					}

					if(isRotatePressed)
					{
						Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
						var objectPos = Camera.main.WorldToScreenPoint (this.m_SelectedItemObj.transform.position);
						var dir = movePos - objectPos; 
						this.m_SelectedItemObj.transform.rotation =  Quaternion.AngleAxis ( Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - rotateOffset + orgRotation.eulerAngles.z, 
						                                            Vector3.forward) ;
						
						
					}
					if(isScalePressed)
					{
						Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
						var objectPos = Camera.main.WorldToScreenPoint (this.m_SelectedItemObj.transform.position);
						float distance = Vector3.Distance(movePos,objectPos);
						float dir = distance/distanceOffset; 
						//		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg));
						this.m_SelectedItemObj.transform.localScale = orgScale * dir;
					}

				}
			}

			if (Input.GetMouseButtonUp (0)) {

				if(isLongPressd)
				{
					if(this.m_SelectedItemObj != null)
					{
						this.m_SelectedItemObj.GetComponent<TextItemController>().setInputActive();
					}
					StopAllCoroutines();
					isLongPressd = false;
				}
				isRotatePressed = false;
				isMoveItem = false;
				isScalePressed = false;
				isPressed = false;
			}
		}
	}

	IEnumerator startClicked(float times)
	{
		yield return new WaitForSeconds (times);
		isLongPressd = false;
	}

	public void AddStickerItem(TextItemController item)
	{
		if (item != null) {
			m_textItemControllerList.Add(item);
		}
	}

	public void RemoveStickerItem(TextItemController item)
	{
		if (item != null) {
			m_textItemControllerList.Remove(item);
		}

		if (m_textItemControllerList.Count == 0) {
			m_isNeedApply = false;
		}
	}

	public void ClearAll()
	{
		for (int i = 0; i!= m_textItemControllerList.Count; i++) {
			Destroy(m_textItemControllerList[i].gameObject);
		}
		
		m_textItemControllerList.Clear ();
	}

	public void CreateItem()
	{
		if (instanceTextObj != null) {
			GameObject obj = Instantiate(instanceTextObj,this.transform.position,Quaternion.identity) as GameObject;
			obj.transform.parent = this.transform;
			
			AddStickerItem(obj.GetComponent<TextItemController>());
			
			obj.GetComponent<TextItemController>().initItem();
			obj.GetComponent<TextItemController>().setTextManager(this);
			if(this.m_SelectedItemObj != null)
			{
				this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
			}
			m_SelectedItemObj = obj.transform;
			m_isNeedApply = true;
		}
	}

	bool isWork = false;
	public void setWork(bool value)
	{
		isWork = value;
		m_isNeedApply = false;
		if (this.m_SelectedItemObj != null) {
			this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
		}
	}

	public void sharePanelWork(bool value)
	{
		if (value) {
			if (this.m_SelectedItemObj != null) {
				this.m_SelectedItemObj.GetComponent<TextItemController>().setActive(false);
			}
		}
	}

	public void colorSliderChanged()
	{
		float value = e_ColorSlider.value * 22f;
		Color tempColor = Color.white;
		if (value > 0 && value < 1) {
			tempColor = colorARR[0];
		} else if (value > 1 && value < 2) {
			tempColor = colorARR[1];
		}else if (value > 2 && value < 3) {
			tempColor = colorARR[2];
		}else if (value > 3 && value < 4) {
			tempColor = colorARR[3];
		}else if (value > 4 && value < 5) {
			tempColor = colorARR[4];
		}else if (value > 5 && value < 6) {
			tempColor = colorARR[5];
		}else if (value > 6 && value < 7) {
			tempColor = colorARR[6];
		}else if (value > 7 && value < 8) {
			tempColor = colorARR[7];
		}else if (value > 8 && value < 9) {
			tempColor = colorARR[8];
		}else if (value > 9 && value < 10) {
			tempColor = colorARR[9];
		}else if (value > 10 && value < 11) {
			tempColor = colorARR[10];
		}else if (value > 11 && value < 12) {
			tempColor = colorARR[11];
		}
		else if (value > 12 && value < 13) {
			tempColor = colorARR[12];
		}else if (value > 13 && value < 14) {
			tempColor = colorARR[13];
		}else if (value > 14 && value < 15) {
			tempColor = colorARR[14];
		}else if (value > 15&& value < 16) {
			tempColor = colorARR[15];
		}else if (value > 16&& value < 17) {
			tempColor = colorARR[16];
		}else if (value > 17 && value < 18) {
			tempColor = colorARR[17];
		}else if (value > 18 && value < 19) {
			tempColor = colorARR[18];
		}
		else if (value > 19 && value < 20) {
			tempColor = colorARR[19];
		}else if (value > 20 && value < 21) {
			tempColor = colorARR[20];
		}else if (value > 21 && value < 22.02) {
			tempColor = colorARR[21];
		}
		if (e_colorSliderHandleObj != null) {
			e_colorSliderHandleObj.color = tempColor;
		}
		if (this.m_SelectedItemObj) {
			this.m_SelectedItemObj.GetComponent<TextItemController>().setTextColor(tempColor);
		}
}

	public void alphaSliderChanged()
	{
		if (e_AlphaSlider != null && m_SelectedItemObj !=null) {
			this.m_SelectedItemObj.GetComponent<TextItemController>().setTextAlpha(e_AlphaSlider.value);
		}
	}
	
	void onComboBoxSelected(int index)
	{
		if (this.m_SelectedItemObj != null && index < fontARR.Length) {
			this.m_SelectedItemObj.GetComponent<TextItemController>().setTextText(fontARR[index]);
		}
	}

	void ColorSliderDown(GameObject obj)
	{
		e_colorSliderHandleObj.DOFade (1, 0.6f);
	}

	void ColorSliderUp(GameObject obj)
	{
		e_colorSliderHandleObj.DOFade (0, 1f);
	}
}
