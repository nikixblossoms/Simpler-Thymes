using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LoopBG : MonoBehaviour
{
    public float loopSpeed;
    public Renderer bgRenderer;

    // Update is called once per frame
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(loopSpeed * Time.deltaTime, 0f);   
    }
}
