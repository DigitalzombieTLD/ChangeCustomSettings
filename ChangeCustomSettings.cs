using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using Il2Cpp;
using System.Reflection;
using UnityEngine.EventSystems;
using Il2CppTLD.Gameplay;


namespace ChangeCustomSettings
{
	public class ChangeCustomSettingsMain : MelonMod
	{
        public static EventSystem eventSystem;

        public override void OnInitializeMelon()
        {
            
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
            if ((!sceneName.Contains("Empty") && !sceneName.Contains("Boot") && sceneName.Contains("MainMenu")))
            {
                if (!eventSystem)
                {
                    Camera eventCam = GameManager.GetMainCamera();

                    eventSystem = eventCam.gameObject.GetComponent<EventSystem>();

                    if (!eventSystem)
                    {
                        eventSystem = eventCam.gameObject.AddComponent<EventSystem>();
                        eventCam.gameObject.AddComponent<StandaloneInputModule>();
                    }                  
                }
            }
        }

        public override void OnUpdate()
        {       
        }
    }
}