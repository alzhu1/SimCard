using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

[ExecuteInEditMode]
public class SpritePositionEditor : MonoBehaviour {
    [SerializeField] private Texture2D texture;

    void Update() {
        // string path = AssetDatabase.GetAssetPath(texture);
        // TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        // ti.isReadable = true;

        Debug.Log("Running now");

        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texture);
        dataProvider.InitSpriteEditorDataProvider();

        /* Use the data provider */
        var a = dataProvider.GetSpriteRects();
        Debug.Log(a.Length);

        foreach (var aa in a) {
            // Debug.Log(aa.rect);
            aa.rect = new Rect(
                aa.rect.x,
                aa.rect.y + 1,
                aa.rect.width,
                aa.rect.height
            );
            // break;
        }
        dataProvider.SetSpriteRects(a);

        // Apply the changes made to the data provider
        dataProvider.Apply();

        // Reimport the asset to have the changes applied
        var assetImporter = dataProvider.targetObject as AssetImporter;
        assetImporter.SaveAndReimport();

        // List < SpriteMetaData > newData = new List < SpriteMetaData > ();
        // for (int i = 0; i < ti.spritesheet.Length; i++) {
        //     SpriteMetaData d = ti.spritesheet[i];

        //     //do whatever you want with the metadata...
        //     Debug.Log($"The metadata: {d}");

        //     newData.Add(d);
        // }
        // ti.spritesheet = newData.ToArray();
        // AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        enabled = false;
    }
}
