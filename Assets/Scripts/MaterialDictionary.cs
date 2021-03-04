using System.IO;
using UnityEditor;
using UnityEngine;

public class MaterialDictionary : MonoBehaviour
{
    public string materialDataDirectory;
    public bool reloadData;
    public MaterialData[] materialDatas;

    public string GetNameFromAlias(string alias)
    {
        foreach(MaterialData materialData in materialDatas)
        {
            foreach(string entry in materialData.alias)
            {
                if (entry == alias)
                    return materialData.displayName;
            }
        }
        return null;
    }
    public string GetDisplayName(string name)
    {
        foreach (MaterialData materialData in materialDatas)
        {
            if (materialData.displayName == name)
            {
                return materialData.displayName;
            }
        }
        return null;
    }
    public string GetTextureName(string name)
    {
        foreach (MaterialData materialData in materialDatas)
        {
            if (materialData.displayName == name)
            {
                return materialData.texture;
            }
        }
        return null;
    }

    void OnValidate()
    {
        if (reloadData == true)
        {
            string[] files = Directory.GetFiles("Assets/GameData/materials", "*.json");
            materialDatas = new MaterialData[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(files[i], typeof(TextAsset));
                materialDatas[i] = JsonUtility.FromJson<MaterialData>(textAsset.text);

            }
        }
        reloadData = false;
    }
}
