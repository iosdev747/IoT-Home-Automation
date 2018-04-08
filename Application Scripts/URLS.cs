using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class URLS : MonoBehaviour {

	public Item Modules;

	public void executeURL(Text uid)
	{
		loadData();
		int i = int.Parse (uid.text);
		Debug.Log (i);
		Modules.state [i] = !Modules.state [i];
		Debug.Log (Modules.Url [i]);
		string urltoexecute;
		urltoexecute = PlayerPrefs.GetString ("url") + "/iosdev/" + Modules.Url [i]; 
		urltoexecute += (Modules.state [i]?"1/":"0/");
		if (Modules.state [i]) {
			PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : " + Modules.ModuleName [i] + " >>> " + "On");
		} else {
			PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : " + Modules.ModuleName [i] + " >>> " + "Off");
		}
		saveData ();
		WWW www = new WWW (urltoexecute);
		Debug.Log (urltoexecute);
	}

	public void removeacc(Text uid)
	{
		loadData();
		int i = int.Parse (uid.text);
		Modules.ModuleName [i] = "";
		Modules.state [i] = false;
		Modules.UID [i] = 0;
		Modules.Url [i] = "";
		saveData ();
		SceneManager.LoadScene ("main");
	}

	public void saveData()
	{
		BinaryFormatter b = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/saves.dat");
		b.Serialize (file, Modules);
		file.Close ();
		Debug.Log ("DATA SAVED");
	}

	public void loadData()
	{
		if (File.Exists (Application.persistentDataPath + "/saves.dat")) 
		{
			BinaryFormatter b = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/saves.dat", FileMode.Open);
			Modules = (Item)b.Deserialize(file);
			file.Close ();
			Debug.Log ("DATA LOADED");
		}
	}
}
