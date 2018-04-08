using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	public Button buttonComponent;
	public Text moduleName;
	public Text UID;
	public Text url;
	public Text state;
	private ScrollManager scrollList;
	public Sprite Image1;
	public Sprite Image2;
	public Image image;
	private Item item;
	void Start(){
	
	}

	public void Setup(Item currentItem, ScrollManager currentScrollList, int uid, imgbutton img){
		item = currentItem;
		moduleName.text = currentItem.ModuleName[uid];
		Debug.Log ("SETUP");
		Debug.Log (currentItem.ModuleName [uid]);	
		UID.text = currentItem.UID[uid].ToString ();
		url.text = currentItem.Url[uid];
		Debug.Log (url.text);
		Debug.Log("done");
		state.text = (currentItem.state[uid]?"true":"false");
		scrollList = currentScrollList;
	}

	void Update()
	{
		int i = int.Parse (UID.text);
		loadData ();
		if (item.state [i]) {
			image.sprite = Image1;
			Debug.Log (i + "on");
		}
		else{
			image.sprite = Image2;
			Debug.Log (i + "off");
		}

	}

	public void loadData()
	{
		if (File.Exists (Application.persistentDataPath + "/saves.dat")) 
		{
			BinaryFormatter b = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/saves.dat", FileMode.Open);
			item = (Item)b.Deserialize(file);
			file.Close ();
			Debug.Log ("DATA LOADED");
		}
	}
}



/*
	// Use this for initialization
	void Start () {
		buttonComponent.onClick.AddListener (HandleClick);
	}
	
	public void Setup(Item currentItem, ScrollManager currentScrollList)
	{
		

	}

	public void HandleClick()
	{
		scrollList.TryTransferItemToOtherShop (item);
	}
}*/