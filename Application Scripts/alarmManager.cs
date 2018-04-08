using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class alarmManager : MonoBehaviour {

	public InputField name;
	public InputField hh;
	public InputField mm;
	public InputField ss;
	private Item Modules;
	public void actionPerform(bool state)
	{
		loadData ();
		for (int i = 1; i < 99; i++) {
			Debug.Log (Modules.ModuleName [i] + name.text);
			if (Modules.ModuleName [i] == name.text) {
				string url = PlayerPrefs.GetString("url");
				url += "/salarm/";
				url += Modules.Url[i];
				if (state) {
					url += "1";
					Modules.state [i] = true;
				} else {
					url += "0";
					Modules.state [i] = false;
				}
				long millis = tomillis (int.Parse(hh.text),int.Parse(mm.text),int.Parse(ss.text));
				url += millis;
				url += "/";
				PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : Set Alarm for " + Modules.ModuleName[i] + " at " + hh.text + ":" + mm.text + ":" + ss.text);
				WWW www = new WWW (url);
				Debug.Log (url);
				saveData ();
				SceneManager.LoadScene("main");
			}
		}
	}

	public long tomillis(int hh, int mm, int ss)
	{
		long millis = 0;
		millis += hh * 3600000;
		millis += mm * 60000;
		millis += ss * 1000;
		return millis;
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
