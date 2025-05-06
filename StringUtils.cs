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
	public static class StringUtils
	{
        public static string AddDashes(string str)
        {
            if (str.Contains("-"))
            {
               return str;
            }

            string result = "";

            for (int i = 0; i < str.Length; i++)
            {
                result += str[i];
                if ((i + 1) % 4 == 0 && i != str.Length - 1)
                {
                    result += "-";
                }
            }

            return result;
        }

        public static string RemoveDashes(string str)
        {
            if (!str.Contains("-"))
            {
                return str;
            }

            string result = "";

            for (int i = 0; i < str.Length; i++)
            {
                if(str[i] != '-')
                {
                    result += str[i];
                }
            }

            return result;
        }
    }
}