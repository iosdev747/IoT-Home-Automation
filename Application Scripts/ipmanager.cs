using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ipmanager : MonoBehaviour {

	public InputField ipfield;

	public void NextScene(string s)
	{
		PlayerPrefs.SetString ("url",ipfield.text);
		SceneManager.LoadScene(s);
	}
}
