using TexDrawLib;

using UnityEngine;
using UnityEngine.UI;

// This component is invisible in scene, but plays a vital role rendering the fonts.
[ExecuteAlways, RequireComponent(typeof(CanvasRenderer))]
public class TEXDrawRenderer : MaskableGraphic, ITexRenderer
{
    public TEXDraw m_TEXDraw;
    public int m_FontMode = -1;
    public ITEXDraw TEXDraw => m_TEXDraw ?? (m_TEXDraw = GetComponentInParent<TEXDraw>());
    public int FontMode
    {
        get => m_FontMode; set
        {
            if (m_FontMode != value)
            {
                m_FontMode = value;
                canvasRenderer.SetTexture(mainTexture);
            }
        }
    }

    protected override void UpdateGeometry()
    {
        if (m_FontMode == -1 || !m_TEXDraw)
            workerMesh.Clear();
        else
        {
            m_TEXDraw.orchestrator.rendererState
                .GetVertexForFont(m_FontMode)
                ?.FillMesh(workerMesh, false);
        }
        canvasRenderer.SetMesh(workerMesh);
    }

    public override Texture mainTexture => m_FontMode >= 0 ?
        TEXDraw?.preference.fonts[m_FontMode].Texture() : s_WhiteTexture;

    public override Material defaultMaterial => m_TEXDraw?.defaultMaterial;

}
