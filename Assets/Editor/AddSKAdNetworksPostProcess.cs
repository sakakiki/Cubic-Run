#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class AddSKAdNetworksPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;

        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementArray skAdItems = plist.root["SKAdNetworkItems"] as PlistElementArray;
        if (skAdItems == null)
        {
            skAdItems = plist.root.CreateArray("SKAdNetworkItems");
        }

        string[] ids = new string[]
        {
            "f38h382jlk.skadnetwork", // Chartboost
            "t38b2kh725.skadnetwork", // LifeStreet Media
            "44jx6755aq.skadnetwork", // Persona.ly Ltd.
            "k674qkevps.skadnetwork", // Pubmatic
            "klf5c3l5u5.skadnetwork", // Sift Media
            "4w7y6s5ca2.skadnetwork", // StackAdapt
            "mlmmfzh3r3.skadnetwork", // Viant
            "c6k4g5qg8m.skadnetwork"  // Zemanta
        };

        foreach (string id in ids)
        {
            bool exists = false;
            foreach (var item in skAdItems.values)
            {
                if (item is PlistElementDict dict && dict["SKAdNetworkIdentifier"].AsString() == id)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                var dict = skAdItems.AddDict();
                dict.SetString("SKAdNetworkIdentifier", id);
            }
        }

        plist.WriteToFile(plistPath);
    }
}
#endif