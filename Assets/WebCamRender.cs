using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamRender : MonoBehaviour
{
   public string deviceName;
    public Image overlay;

   WebCamTexture wct;
   
   // Use this for initialization
   void Start () {
     WebCamDevice[] devices = WebCamTexture.devices;
     deviceName = devices[0].name;
     wct = new WebCamTexture(deviceName, 400, 300, 12);
     GetComponent<Renderer>().material.mainTexture = wct;
     wct.Play();
   }
   
   // For photo varibles
   
   public Texture2D heightmap;
   public Vector3 size = new Vector3(100, 10, 100);
   
   
   void OnGUI() {      
     if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
       TakeSnapshot();
     
   }
   
   // For saving to the _savepath
   private string _SavePath = "Face" ; //Change the path here!
   int _CaptureCounter = 0;
 
   void TakeSnapshot()
     {
       Texture2D snap = new Texture2D(wct.width, wct.height);
       snap.SetPixels(wct.GetPixels());
       snap.Apply();
 
       System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", snap.EncodeToPNG());
       ++_CaptureCounter;
   }
 }

