using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Dialogue")]
public class Speech : ScriptableObject

{
[TextArea]
    public List<string> textList = new List<string>();
    public Vector3 position;

    public Vector3 size;
    public List<Sprite> spriteList = new List<Sprite>();

    public Rect GetRect()
    {
        return new Rect(position.x, position.y, size.x, size.y);
    }

}