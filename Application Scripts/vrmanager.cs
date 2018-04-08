using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.XR;

public class vrmanager : MonoBehaviour {

	public void NextScene(string s)
	{
		SceneManager.LoadScene(s);
	}
	IEnumerator LoadDevice(string newDevice, bool enable)
	{
		XRSettings.LoadDeviceByName(newDevice);
		yield return null;
		XRSettings.enabled = enable;
	}

	void Start()//Update()
	{
		StartCoroutine(LoadDevice("cardboard", true));
	}

	public void EnableVR(string s)
	{
		StartCoroutine(LoadDevice("cardboard", true));
		SceneManager.LoadScene(s);
	}

	public void DisableVR(string s)
	{
		StartCoroutine(LoadDevice("", false));
		SceneManager.LoadScene(s);
	}
}
