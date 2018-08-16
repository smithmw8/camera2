using UnityEngine;
using System.Collections;
using CWALK;

public class WebCameraController : MonoBehaviour {

	public WebCamTexture cameraTexture;  
	private string cameraName="";  
	private bool m_isPlay = false;  
	public bool isPlay
	{
		get
		{
			return m_isPlay;
		}
	}
	
	public Material[] matArr;
	CameraMode m_CameraMode = CameraMode.DEFAULT;
	public CameraMode cameramode
	{
		get
		{
			return m_CameraMode;
		}
	}

	// Use this for initialization  
	void Start()  
	{
        WebCamDevice[] devices = WebCamTexture.devices;

        string faceName = "";
        string backtName = "";
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                faceName = devices[i].name;
            }
            else
            {
                backtName = devices[i].name;
            }
        }


        if (!string.IsNullOrEmpty(backtName) &&
           (m_CameraMode == CameraMode.BACK))
        {
            cameraName = backtName;
        }
        else if (!string.IsNullOrEmpty(faceName))
        {
            m_CameraMode = CameraMode.FRONT;
            cameraName = faceName;
        }

        cameraTexture = new WebCamTexture(cameraName, 480, 640, 15);
        cameraTexture.Play();
        m_isPlay = true;

//        StartCoroutine(startDeviceCamera());  
	}

	IEnumerator startDeviceCamera()  
	{  
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);  
		if (Application.HasUserAuthorization(UserAuthorization.WebCam))  
		{  
			WebCamDevice[] devices = WebCamTexture.devices;  

			string faceName = "";
			string backtName = "";
			for(int i = 0;i<devices.Length;i++)
			{
				if(devices[i].isFrontFacing)
				{
					faceName = devices[i].name;
				}
				else
				{
					backtName = devices[i].name;
				}
			}

		
			if(!string.IsNullOrEmpty( backtName) && 
			   (m_CameraMode == CameraMode.BACK))
			{
				cameraName = backtName; 
			}
			else if(!string.IsNullOrEmpty( faceName))
			{
				m_CameraMode = CameraMode.FRONT;
				cameraName = faceName;  
			}

			cameraTexture = new WebCamTexture(cameraName, 480, 640, 15);  
			cameraTexture.Play();
			m_isPlay = true;  
		}  
	}
	
	public void ChangeDeviceCameraMode()
	{
		if (m_CameraMode == CameraMode.DEFAULT ||
			m_CameraMode == CameraMode.FRONT) {
			m_CameraMode = CameraMode.BACK;
		} else {
			m_CameraMode = CameraMode.FRONT;
		}

		this.m_isPlay = false;
		this.cameraTexture.Stop();
		StartCoroutine(startDeviceCamera());
		AppManager.instance.changeCameraMode (m_CameraMode);
	}

	public void StopWork()
	{
		this.cameraTexture.Stop();
	}

	public void setCameraFreeze()
	{
		if (cameraTexture.isPlaying) {

			#if UNITY_IOS
			this.cameraTexture.Pause();
			#else
			this.cameraTexture.Stop();
			#endif
			this.m_isPlay = false;
		}
	}

	public void setCameraActive()
	{
		if (!cameraTexture.isPlaying) {
			this.cameraTexture.Play();
			this.m_isPlay = true;
		}
	}
}

