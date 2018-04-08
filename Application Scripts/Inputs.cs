using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class Inputs : MonoBehaviour {

	public InputField moduleName;
	public InputField moduleUrl;
	public Item Modules;

	public void buttonPressed()
	{
		loadData ();
		string modulename, moduleurl="";
		modulename = moduleName.text;
		moduleurl = moduleUrl.text;
		Debug.Log (modulename+moduleurl);
		for (int i = 1; i<99 ; i++)
		{
			Debug.Log (i);
			if (Modules.UID [i] == 0 && i!=0) 
			{
				Modules.UID [i] = i;
				Modules.ModuleName[i] = modulename;
				Modules.Url[i] = moduleurl;
				break;
			}
		}
		saveData ();
		SceneManager.LoadScene("main");
	}

	/*public static int IntParseFast(string value)
	{
		int result = 0;
		for (int i = 1; i < value.Length; i++)
		{
			char letter = value[i];
			result = 10 * result + (letter - 48);
		}
		return result;
	}*/

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
