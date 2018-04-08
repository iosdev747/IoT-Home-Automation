using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class Refreshacc : MonoBehaviour {

	public Item Modules;
	public Text lastAction;
	public void refresh()
	{
		loadData ();
		string url = PlayerPrefs.GetString("url");
		url += "/access/";
		for (int i = 1; i<99 ; i++)
		{
			if (Modules.UID [i] != 0 || i==0) 
			{
				url += Modules.ModuleName[i];
				url += "/";
				url += Modules.Url[i];
				url += Modules.state [i] ? "1/" : "0/";
				PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : Refresh Accessories");
			}
		}
		WWW www = new WWW (url);
		Debug.Log (url);
		Debug.Log (System.DateTime.Today.Month);
		Debug.Log (System.DateTime.Today.Year);
		Debug.Log (System.DateTime.Today.Day);
		string url2 = PlayerPrefs.GetString("url");
		url2 += "/setime/";
		url2 += System.DateTime.Today.Day + "0" + System.DateTime.Today.Month + System.DateTime.Today.Year;
		url2 += tomillis(System.DateTime.Now.Hour,System.DateTime.Now.Minute,System.DateTime.Now.Second);
		WWW www2 = new WWW (url2);
		Debug.Log (url2);
	}

	public long tomillis(int hh, int mm, int ss)
	{
		long millis = 0;
		millis += hh * 3600000;
		millis += mm * 60000;
		millis += ss * 1000;
		return millis;
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

	void Update()
	{
		lastAction.text = PlayerPrefs.GetString("lastAction");
	}
}
