using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

[ExecuteInEditMode]
public class SpritePositionEditor : MonoBehaviour {
    [SerializeField] private Texture2D texture;

    void Update() {
        Debug.Log("Running now");

        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texture);
        dataProvider.InitSpriteEditorDataProvider();

        /* Use the data provider */
        var a = dataProvider.GetSpriteRects();
        Debug.Log(a.Length);

        foreach (var aa in a) {
            aa.rect = new Rect(
                aa.rect.x + 1,
                aa.rect.y,
                aa.rect.width,
                aa.rect.height
            );
        }
        dataProvider.SetSpriteRects(a);

        // Apply the changes made to the data provider
        dataProvider.Apply();

        // Reimport the asset to have the changes applied
        var assetImporter = dataProvider.targetObject as AssetImporter;
        assetImporter.SaveAndReimport();

        enabled = false;
    }
}
