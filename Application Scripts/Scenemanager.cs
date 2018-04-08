using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Scenemanager : MonoBehaviour {

	public void NextScene(string s)
	{
		SceneManager.LoadScene(s);
	}

	public void quit(bool b)
	{
		if (b) {
			Application.Quit ();
		}
	}
}
