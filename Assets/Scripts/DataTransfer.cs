using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Text;
using Newtonsoft.Json;


public class DataTransfer : MonoBehaviour
{
    
    public InputField bodyMessage;
    public InputField recipientEmail;
    public Text counterObj;

	public Text surveyID;

    public Text noneTimer;
    public Text glassTimer;
    public Text sharkTimer;
    public Text cloudTimer;

    public Text touchCount;
    public Text touchCountNoFilter;
    public Text touchCountFilter1;
    public Text touchCountFilter2;
    public Text touchCountFilter3;

    public Text mailMessage;
	
    private readonly string basePath = "http://167.71.95.235/experiments";

    public class Data
    {
        public string name { get; set; }
        public int numberOfClicks { get; set; }
        public int timeSpent { get; set; }
        public string id { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }

    public class Root
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class DataBox
    {
        public int numberOfClicks { get; set; }
        public int timeSpent { get; set; }
    }

	void Start() 
	{
		StartCoroutine(Post(basePath));
	}
    // Start is called before the first frame update
    public void WriteDataToDB()
    {
		//Put();
		//SceneManager.LoadScene(3);
    }

	IEnumerator Post(string basePath)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest ww = UnityWebRequest.Post(basePath, form);
        
        yield return ww.SendWebRequest();

		if(ww.isNetworkError || ww.isHttpError) {
            Debug.Log(ww.error);
        }
        else {
            // Show results as text
            Debug.Log(ww.downloadHandler.text);

			Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(ww.downloadHandler.text); 

			surveyID.text = myDeserializedClass.data.id;

			Debug.Log(myDeserializedClass.data.id);
 
		}

		StartCoroutine(Put());
		
	}

	IEnumerator Put()
    {
		string path = "http://167.71.95.235/experiments/:" + surveyID.text;
        
        DataBox box = new DataBox();
		box.numberOfClicks = 20;
		box.timeSpent = 30;
        string jsonData = JsonConvert.SerializeObject(box);
        //string jsonData = JsonUtility.ToJson(box);

        using (UnityWebRequest www = UnityWebRequest.Put(path, jsonData))
        {
            www.method = "PUT";
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log((www.downloadHandler.text));
            }
        }
    } 
    
}
