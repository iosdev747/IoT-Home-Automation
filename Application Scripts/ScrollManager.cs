using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Item
{
	public int[] UID = new int[99];
	public string[] ModuleName = new string[99];
	//public Sprite[] Image = new Sprite[99];
	//public Sprite[] StateImage = new Sprite[99];
	public string[] Url = new string[99];
	public bool[] state = new bool[99];
}
public class imgbutton
{
	public Image[] image = new Image[99];
}

public class ScrollManager : MonoBehaviour {

	public Item Modules;
	public imgbutton image;
	public Transform contentPanel;		//check if can remove/// / / / / / / / / / / / / / / / / / / / / / / / / / / /
	public ObjectPool ObjectPool_Switch;

	// Use this for initialization
	void Start () 
	{
		loadData ();
		RefreshDisplay ();
	}

	public void RefreshDisplay()
	{
		//RemoveButtons ();
		var clones = GameObject.FindGameObjectsWithTag ("Accessory"); foreach (var clone in clones){ Destroy(clone); }
		AddButtons ();
	}

//	private void RemoveButtons()
//	{
//		while (contentPanel.childCount > 0) 
//		{
//			GameObject toRemove = transform.GetChild(0).gameObject;
//			buttonObjectPool.ReturnObject(toRemove);
//		}
//	}

	private void AddButtons()
	{
		int i;
		for (i = 1; i < 99; i++) {
			if (Modules.UID [i] != 0) {
				GameObject newButton = ObjectPool_Switch.GetObject ();
				newButton.transform.SetParent (contentPanel);

				ButtonController Button = newButton.GetComponent<ButtonController> ();
				Button.Setup (Modules, this, i, image);
			}
		}
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
			Debug.Log (Application.persistentDataPath);
			Debug.Log ("DATA LOADED");
		}
	}
}