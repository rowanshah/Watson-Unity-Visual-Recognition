using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.VisualRecognition.V3.Model;
using static IBM.Cloud.SDK.Constants;
using static System.Net.WebRequestMethods;
using Utility = IBM.Cloud.SDK.Utilities.Utility;
using IBM.Cloud.SDK.Authentication;
using Boo.Lang;

namespace IBM.Watson.VisualRecognition.V3 {
    public class FaceExpRecognition :  MonoBehaviour {  
         
         #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("https://gateway.watsonplatform.net/visual-recognition/api")]
        [SerializeField]
        private string _serviceUrl;
        [Tooltip("Text field to display the results of streaming.")]
        public Text ResultsField;
        [Header("IAM Authentication")]
        [Tooltip("Please Enter your API key here")]
        [SerializeField]
        private string _iamApikey = "Please Enter your API key here";
        [Header("Parameters")]
        [Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]
        [SerializeField]

        public System.Collections.Generic.List<string> myList = new System.Collections.Generic.List<string> { "Please enter your model ID here"};

        public VisualRecognitionService service;
        private string picUrl = @"C:\\users\rowanshah\Documents\GitHub\Unity-Watson-VisualRecognition\Unity-Watson";
        public BaseService _baseService;
        private string turtleImageContentType = "images/jpeg";
        private string turtleImageFilepath = "Face1.png" ;
        private const string defaultUrl = "https://gateway.watsonplatform.net/visual-recognition/api";

        public WebCamRender wcr; 

        #endregion     

        #region DisableSslVerification
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        #region VersionDate
        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
        }
        #endregion

         #region Credentials
        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return _baseService.credentials; }
            set
            {
                _baseService.credentials = value;
                if (!string.IsNullOrEmpty(_baseService.credentials.Url))
                {
                    Url = _baseService.credentials.Url;
                }
            }
        }
        #endregion

        #region Url
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _baseService.url; }
            set { _baseService.url = value; }
        }
        #endregion
        

    
        // Start is called before the first frame update
        void Start() {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(TestClassify());
        }


        public IEnumerator TestClassify()
        {
            //  Create credential and instantiate service
            Credentials credentials = null;

            //  Authenticate using iamApikey
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = _iamApikey
            };

            credentials = new Credentials(tokenOptions, _serviceUrl);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

             if (string.IsNullOrEmpty(_iamApikey))
            {
                throw new IBMException("Plesae provide IAM ApiKey for the service.");
            }
            service = new VisualRecognitionService("2018-03-19", credentials);
           // service.StreamMultipart = true;

            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to Classify...");
            ClassifiedImages classifyResponse = null;
            using (FileStream fs = System.IO.File.OpenRead(turtleImageFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.Classify(
                    callback: (DetailedResponse<ClassifiedImages> response, IBMError error) =>
                    {
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Classify result: {0}", response.Response);
                        classifyResponse = response.Result;
                    },
                    imagesFile: ms,
                    imagesFilename: System.IO.Path.GetFileName(turtleImageFilepath),
                    imagesFileContentType: turtleImageContentType,
                    url: picUrl,
                    classifierIds: myList
                );

                    while (classifyResponse == null)
                        yield return null;
                }
            }
        }
         public void VisualRecognitionService(string versionDate, Credentials credentials)
        {
            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of VisualRecognitionService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (credentials.HasCredentials() || credentials.HasTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = defaultUrl;
                }
            }
            else
            {
                throw new IBMException("Please provide a username and password or authorization token to use the VisualRecognition service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
       
    }
}


