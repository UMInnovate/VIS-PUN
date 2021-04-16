
using UnityEngine;

[ExecuteAlways]
public class TEXDrawRendererFactory : MonoBehaviour
{
    public void Start()
    {
        var tex = GetComponentInParent<TEXDraw>();
        if (tex)
        {
            var ren = gameObject.AddComponent<TEXDrawRenderer>();
            tex.RegisterRenderer(ren);
            tex.SetVerticesDirty();
            ren.m_TEXDraw = tex;
        }
        if (Application.IsPlaying(this))
        {
            Destroy(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

}