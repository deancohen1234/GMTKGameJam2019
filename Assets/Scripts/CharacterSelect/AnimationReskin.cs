using System;
using UnityEngine;

public class AnimationReskin : MonoBehaviour
{
    public string m_SpriteSheetName;

    private void Start()
    {
        var allSubSprites = Resources.LoadAll<Sprite>("Characters/" + m_SpriteSheetName);

        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            string spriteName = renderer.sprite.name;
            var newSprite = Array.Find(allSubSprites, item => item.name == spriteName);

            if (newSprite)
            {
                renderer.sprite = newSprite;
                Debug.Log(renderer.sprite.name);
            }
        }
    }

    void LateUpdate()
    {
        
        var allSubSprites = Resources.LoadAll<Sprite>("Characters/" + m_SpriteSheetName);

        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            string spriteName = renderer.sprite.name;
            Debug.Log("Sprite Name: " + spriteName);

            var newSprite = Array.Find(allSubSprites, item => item.name == spriteName);

            if (newSprite)
            {
                renderer.sprite = newSprite;
            }
        }
        
    }
}
