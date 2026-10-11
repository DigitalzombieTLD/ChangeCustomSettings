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
                if (copyButtonPrefab == null)
                {
                    return;
                }

                SharingStringView = this.transform.Find("SharingStringView").gameObject;
                ShareStringLabel = SharingStringView.transform.Find("ShareStringLabel").gameObject;
             
                copyButtonInstance = GameObject.Instantiate(copyButtonPrefab);
                copyButtonInstance.transform.SetParent(SharingStringView.transform, false);
          
                copyButtonInstance.name = "CopyButtonParent";
                copyButtonInstance.SetActive(true);
                AddLabel(copyButtonInstance.transform);
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
                    if (labelImage != null && copySprite != null)
                    {
                        labelImage.sprite = copySprite;
                    }
                    currentTimer = 0f;
                    timerActive = false;
                }
            }
        }

        public void ShowText()
        {
            copyText.SetActive(true);
            if (labelImage != null && copiedSprite != null)
            {
                labelImage.sprite = copiedSprite;
            }
            currentTimer = 0f;
            timerActive = true;
        }

        // The bundle's labels use TextMeshPro, which TLD 2.55 no longer ships (and UnityEngine.UI.Text is
        // stripped from the game), so they show up blank. The label is drawn from a small bitmap instead.
        // The bitmap is embedded as raw 8-bit alpha (no PNG decoding needed - ImageConversion is unreliable here).
        private const int LabelWidth = 240;
        private const int LabelHeight = 72;
        private static Sprite? copySprite;
        private static Sprite? copiedSprite;
        private Image? labelImage;

        private void AddLabel(Transform root)
        {
            try
            {
                Transform? button = FindDeep(root, "Button");
                if (button == null)
                {
                    MelonLogger.Warning("Copy button label: 'Button' object not found in the prefab.");
                    return;
                }

                copySprite ??= LoadSprite("ChangeCustomSettings.Resources.copy.a8");
                copiedSprite ??= LoadSprite("ChangeCustomSettings.Resources.copied.a8");
                if (copySprite == null)
                {
                    MelonLogger.Warning("Copy button label: could not build the label sprite.");
                    return;
                }

                GameObject labelObject = new GameObject("CopyLabel");
                labelObject.layer = button.gameObject.layer; // the UI camera only renders the button's layer
                labelObject.transform.SetParent(button, false);
                labelImage = labelObject.AddComponent<Image>();
                labelImage.sprite = copySprite;
                labelImage.color = Color.white;
                labelImage.preserveAspect = true;
                labelImage.raycastTarget = false;

                RectTransform rect = labelObject.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                labelObject.transform.SetAsLastSibling();
                labelObject.SetActive(true);

            }
            catch (Exception e)
            {
                MelonLogger.Warning("Could not add the copy button label: " + e);
            }
        }

        private static Sprite? LoadSprite(string resourceName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                MelonLogger.Warning("Copy button label: embedded resource " + resourceName + " is missing.");
                return null;
            }

            byte[] alpha = new byte[LabelWidth * LabelHeight];
            int read = 0;
            while (read < alpha.Length)
            {
                int n = stream.Read(alpha, read, alpha.Length - read);
                if (n <= 0) break;
                read += n;
            }
            if (read != alpha.Length)
            {
                MelonLogger.Warning("Copy button label: resource " + resourceName + " has the wrong size (" + read + ").");
                return null;
            }

            // White text, alpha from the bitmap. Raw RGBA32 upload - nothing to decode.
            byte[] rgba = new byte[alpha.Length * 4];
            for (int i = 0; i < alpha.Length; i++)
            {
                rgba[i * 4] = 255;
                rgba[i * 4 + 1] = 255;
                rgba[i * 4 + 2] = 255;
                rgba[i * 4 + 3] = alpha[i];
            }

            Texture2D texture = new Texture2D(LabelWidth, LabelHeight, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.LoadRawTextureData(rgba);
            texture.Apply(false, false);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, LabelWidth, LabelHeight), new Vector2(0.5f, 0.5f), 100f);
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        private static Transform? FindDeep(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
                Transform? found = FindDeep(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public void LoadEmbeddedAssetBundle()
        {
            if(assetBundle != null)
            {
                return;
            }
            // AssetBundle.LoadFromMemory is unreliable on TLD 2.55 / Unity 6: the Il2Cpp byte array can be
            // collected before Unity reads it ("Object was garbage collected in IL2CPP domain") or Unity
            // reads freed memory ("Failed to decompress data for the AssetBundle 'Memory'" crash).
            // Writing the embedded bundle to a file in UserData and using LoadFromFile avoids both.
            string bundlePath = Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "ChangeCustomSettings_copybutton.bundle");
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChangeCustomSettings.Resources.changecustomsettings")!)
            {
                if (!File.Exists(bundlePath) || new FileInfo(bundlePath).Length != stream.Length)
                {
                    using FileStream file = File.Create(bundlePath);
                    stream.CopyTo(file);
                }
            }

            assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
            {
                MelonLogger.Warning("Could not load the copy button asset bundle - the copy button will be missing.");
                return;
            }

            copyButtonPrefab = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>("CopyButtonParent"));           

            GameObject.DontDestroyOnLoad(copyButtonPrefab);
            copyButtonPrefab.SetActive(false);
            copyButtonPrefab.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}