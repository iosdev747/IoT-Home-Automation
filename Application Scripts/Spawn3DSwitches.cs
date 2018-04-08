using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class Spawn3DSwitches : MonoBehaviour {

	private Item Modules;
	GameObject[] go = new GameObject[99];
	public GameObject parent;
	public GameObject Switch;
	public Material[] mat = new Material[2];
	int i,j;
	string tagCheck= "SpaceAcc";
	float lockTime= 2.0f;
	public Text f;
	private bool locking= false;
	private float timestamp= 0.0f;
	private RaycastHit hit;

	void Start () {
		loadData ();
		j = -1;
		i = 0;
		for (int k = 1; k < 99; k++) {
			if (Modules.UID [k] != 0) {
				Destroy (go [k]);
				go [k] = Instantiate (Switch, Vector3.zero, Switch.transform.rotation) as GameObject;
				go [k].transform.localPosition = new Vector3 (parent.transform.position.x + j + 0.5f, parent.transform.position.y + i + 0.5f, parent.transform.position.z + 2);
				if (Modules.state [k]) {
					go [k].GetComponent<Renderer> ().material = mat [0];
				} else {
					go [k].GetComponent<Renderer> ().material = mat [1];
				}
				TextMesh text3d = (TextMesh)go [k].transform.GetChild (0).GetComponent ("TextMesh");
				text3d.text = Modules.ModuleName [k];
				i--;
				if (i == -2) {
					j++;
					i = 0;
				}
			}
		}
		f.text = "DEBUG";
	}


	void Update () {
		if (Input.touchCount > 1) 
		{
			SceneManager.LoadScene ("main");
		}
		if (Physics.Raycast(transform.position, transform.forward,out hit) && (hit.transform.tag == tagCheck)) 
		{
			if (!locking) 
			{
				locking = true;
				timestamp = Time.time + lockTime;
				Debug.Log ("Locked");
			}
		}
		else 
		{
			//f.text = "UNLOCKED";
			locking = false;
		}

		if (locking && Time.time >= timestamp) 
		{
			hit.collider.transform.tag = "Selected";
			Debug.Log ("Locked FIXED");
		}
		for (int k = 1; k < 99; k++) {
			if (Modules.UID [k] != 0 && go [k].tag == "Selected") {
				string urltoexecute;
				Modules.state [k] = !Modules.state [k];
				urltoexecute = PlayerPrefs.GetString ("url") + "/iosdev/" + Modules.Url [k];
				urltoexecute += (Modules.state [k]?"1/":"0/");
				if (Modules.state [k]) {
					go [k].GetComponent<Renderer> ().material = mat [0];
					PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : " + Modules.ModuleName[i] + " >>> " + "On");
				} else {
					go [k].GetComponent<Renderer> ().material = mat [1];
					PlayerPrefs.SetString ("lastAction", "Last Action Taken by User : " + Modules.ModuleName[i] + " >>> " + "Off");
				}
				saveData ();
				locking = false;
				f.text = urltoexecute;
				WWW www = new WWW (urltoexecute);
				go[k].tag = "SpaceAcc";
				Debug.Log ("SpaceAcc" + urltoexecute);
			}
		}
	}

	
	//		if (Input.GetKey (KeyCode.Q)) {
	//			GameObject block = Instantiate(Switch, Vector3.zero, Switch.transform.rotation) as GameObject;
	//			block.transform.localPosition = new Vector3 (parent.transform.position.x + j, parent.transform.position.y + i, parent.transform.position.z + 6);
	//			j++;
	//			if (j >= 2) {
	//				i++;
	//				j = -1;
	//			}

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