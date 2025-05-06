using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using Il2Cpp;
using System.Reflection;
using UnityEngine.UI;
using Il2CppInterop.Runtime;
using UnityEngine.Events;

namespace ChangeCustomSettings
{
    [RegisterTypeInIl2Cpp]
    public class CopyButton : MonoBehaviour
	{
        //public static CopyButton Instance;
        public static AssetBundle assetBundle;
        public static GameObject copyButtonPrefab;
        GameObject copyButtonInstance;

        GameObject SharingStringView;
        GameObject ShareStringLabel;
        UILabel stringLabel;

        Button button;
        
        GameObject copyText;
        float currentTimer = 0f;
        float maxTimer = 2f;
        bool timerActive = false;
        bool isSetup = false;

        public CopyButton(IntPtr intPtr) : base(intPtr)
        {
        }

        public void Awake()
        {
            if(!isSetup)
            {
                LoadEmbeddedAssetBundle();

                SharingStringView = this.transform.Find("SharingStringView").gameObject;
                ShareStringLabel = SharingStringView.transform.Find("ShareStringLabel").gameObject;
             
                copyButtonInstance = GameObject.Instantiate(copyButtonPrefab);
                copyButtonInstance.transform.SetParent(SharingStringView.transform, false);
          
                copyButtonInstance.name = "CopyButtonParent";
                copyButtonInstance.SetActive(true);
                stringLabel = ShareStringLabel.GetComponent<UILabel>();
      
                button = SharingStringView.transform.Find("CopyButtonParent/CopyButton/Button").gameObject.GetComponent<Button>();
                button.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate { this.CopyCustomString(); })));
              
                copyText = SharingStringView.transform.Find("CopyButtonParent/CopyButton/Text").gameObject;
                copyText.SetActive(false);

                isSetup = true;
            }
            copyText.SetActive(false);
            currentTimer = 0f;            
            timerActive = false;
        }

        public void CopyCustomString()
        {
            string customString = stringLabel.text;

            MelonLogger.Msg("Copied difficulty string to clipboard: " + customString);
            ShowText();
            GUIUtility.systemCopyBuffer = customString;
        }

        public void Update()
        {
            if (timerActive)
            {
                currentTimer += Time.deltaTime;
                if (currentTimer >= maxTimer)
                {
                    copyText.SetActive(false);
                    currentTimer = 0f;
                    timerActive = false;
                }
            }
        }

        public void ShowText()
        {
            copyText.SetActive(true);
            currentTimer = 0f;
            timerActive = true;
        }

        public void LoadEmbeddedAssetBundle()
        {
            if(assetBundle != null)
            {
                return;
            }
            MemoryStream memoryStream;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChangeCustomSettings.Resources.changecustomsettings");
            memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);

            assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
            
            copyButtonPrefab = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>("CopyButtonParent"));           

            GameObject.DontDestroyOnLoad(copyButtonPrefab);
            copyButtonPrefab.SetActive(false);
            copyButtonPrefab.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}