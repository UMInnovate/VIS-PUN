using System;
using System.Collections;
using System.Collections.Generic;
using TexDrawLib;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("TEXDraw/TEXDraw UI", 1), ExecuteAlways]
[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasRenderer))]
#if TEXDRAW_DEBUG
[SelectionBase]
#endif
public class TEXDraw : MaskableGraphic, ITEXDraw, ILayoutElement, ILayoutSelfController
{
    public TEXPreference preference => TEXPreference.main;

    [NonSerialized]
    private bool m_TextDirty = true;

    [NonSerialized]
    private List<TEXDrawRenderer> m_Renderers = new List<TEXDrawRenderer>();

    [NonSerialized]
    private bool m_BoxDirty = true;

    [NonSerialized]
    private bool m_OutputDirty = true;

    [SerializeField]
    private Vector2 m_Alignment = Vector2.one * 0.5f;

    [SerializeField, TextArea(5, 10)]
    private string m_Text = "$$TEXDraw$$";

    [SerializeField]
    private float m_Size = 36f;

    [SerializeField]
    private Rect m_ScrollArea = new Rect();

    [SerializeField]
    private TexRectOffset m_Padding = new TexRectOffset(2, 2, 2, 2);

    public virtual string text
    {
        get => m_Text;
        set
        {
            if (m_Text != value)
            {
                m_Text = value;
                SetTextDirty();
            }
        }
    }

    public virtual float size
    {
        get
        {
            return m_Size;
        }
        set
        {
            if (m_Size != value)
            {
                m_Size = Mathf.Max(value, 0f);
                SetTextDirty();
            }
        }
    }

    public virtual TexRectOffset padding
    {
        get => m_Padding;
        set
        {
            m_Padding = value;
            SetVerticesDirty();
        }
    }

    public virtual Rect scrollArea
    {
        get => m_ScrollArea;
        set
        {
            if (m_ScrollArea != value)
            {
                m_ScrollArea = value;
                if (orchestrator?.InputCanvasSize(rectTransform.rect, m_ScrollArea, m_Padding) ?? false)
                    m_BoxDirty = true;
                m_OutputDirty = true;
                SetVerticesDirty();
            }
        }
    }

    public virtual Vector2 alignment
    {
        get => m_Alignment;
        set
        {
            if (m_Alignment != value)
            {
                m_Alignment = value;
                SetTextDirty();
            }
        }
    }

    public override Color color
    {
        get => base.color;
        set
        {
            if (base.color != value)
            {
                base.color = value;
                SetTextDirty();
            }
        }
    }

    private static void FixCanvas(Canvas c)
    {
        if (c)
            c.additionalShaderChannels
            |= AdditionalCanvasShaderChannels.TexCoord1
            | AdditionalCanvasShaderChannels.TexCoord2;
    }

    protected override void OnEnable()
    {
        if (!preference)
        {
            TEXPreference.Initialize();
        }
        orchestrator = new TexOrchestrator();
        GetComponentsInChildren<TEXDrawRenderer>(true, m_Renderers);
        FixCanvas(canvas);
        base.OnEnable();
        m_OutputDirty = true;
        m_BoxDirty = true;
        m_TextDirty = true;
        Font.textureRebuilt += TextureRebuilted;
    }

    protected override void OnDisable()
    {
        Font.textureRebuilt -= TextureRebuilted;
        foreach (var item in m_Renderers)
            if (item)
                item.FontMode = -1;
        m_Renderers.Clear();
        base.OnDisable();
    }

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        FixCanvas(canvas);
    }

    private void TextureRebuilted(Font obj)
    {
        Invoke("SetVerticesDirty", 0);
    }

    public void RegisterRenderer(TEXDrawRenderer renderer)
    {
        for (int i = 0; i < m_Renderers.Count; i++)
        {
            if (m_Renderers[i] == null)
            {
                m_Renderers[i] = renderer;
                return;
            }
        }
    }

#if UNITY_EDITOR

    [ContextMenu("Open Preference")]
    private void OpenPreference()
    {
        UnityEditor.Selection.activeObject = preference;
    }

    [ContextMenu("Trace Output")]
    private void TraceOutput()
    {
        UnityEditor.EditorGUIUtility.systemCopyBuffer = orchestrator.Trace();
        Debug.Log("The trace output has been copied to clipboard.");
    }

#endif

    #region Engine

    private TexOrchestrator m_orchestrator;

    public TexOrchestrator orchestrator { get => m_orchestrator; private set => m_orchestrator = value; }

    public void SetTextDirty()
    {
        m_OutputDirty = true;
        m_BoxDirty = true;
        m_TextDirty = true;

        SetAllDirty();
    }


    internal void CheckGraphicsDirty()
    {
#if UNITY_EDITOR
        if (preference.editorReloading)
            return;
#endif
        // Three main stages of rendering: Parse, Box, Render.
        if (m_TextDirty)
        {
            orchestrator.initialColor = color;
            orchestrator.initialSize = size;
            orchestrator.pixelsPerInch = TEXConfiguration.main.Document.pixelsPerInch * canvas.scaleFactor;
            orchestrator.alignment = m_Alignment;
            orchestrator.Parse(m_Text);

            if (!m_BoxDirty && orchestrator.InputCanvasSize(rectTransform.rect, m_ScrollArea, m_Padding))
                m_BoxDirty = true;

            m_TextDirty = false;
        }
        if (m_BoxDirty)
        {
            orchestrator.InputCanvasSize(rectTransform.rect, m_ScrollArea, m_Padding);
            orchestrator.Box();
            m_OutputDirty = true;
            m_BoxDirty = false;
        }
        if (m_OutputDirty)
        {
            orchestrator.Render();
            var vertexes = orchestrator.rendererState.vertexes;
            for (int i = vertexes.Count; i < m_Renderers.Count; i++)
            {
                if (m_Renderers[i])
                {
                    m_Renderers[i].FontMode = -1;
                }
            }
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (i >= m_Renderers.Count)
                {
                    m_Renderers.Add(null);
                    CreateNewRenderer();
                }
                else if (m_Renderers[i])
                {
                    m_Renderers[i].FontMode = vertexes[i].m_Font;
                }
            }
            m_OutputDirty = false;
        }
    }

    private void CreateNewRenderer()
    {
        var g = new GameObject("TEXDraw Renderer");
#if TEXDRAW_DEBUG
        g.hideFlags = HideFlags.DontSaveInEditor;
#else
        g.hideFlags = HideFlags.HideAndDontSave;
#endif
        var r = g.AddComponent<RectTransform>();
        r.SetParent(transform, false);
        r.anchorMax = Vector2.one;
        r.anchorMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        r.offsetMin = Vector2.zero;
        g.AddComponent<TEXDrawRendererFactory>();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.EditorApplication.delayCall += () =>
            {
                UnityEditor.SceneView.RepaintAll();
            };
        }
#endif
        StartCoroutine(DelayDirtyCallback());
    }

    private IEnumerator DelayDirtyCallback()
    {
        yield return null;
        SetTextDirty();
    }

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
        foreach (var item in m_Renderers)
        {
            if (item)
            {
                item.rectTransform.position = rectTransform.position;
                item.rectTransform.pivot = rectTransform.pivot;
                item.SetVerticesDirty();
            }
        }
    }

    public override void SetLayoutDirty()
    {
        m_BoxDirty = true;
        m_OutputDirty = true;
        base.SetLayoutDirty();
    }

    public override Material defaultMaterial => preference.defaultMaterial;

    protected override void UpdateGeometry()
    {
        CheckGraphicsDirty();
    }

    private void LateUpdate()
    {

        if (transform.hasChanged)
        {
            m_BoxDirty = true;
            SetVerticesDirty();
            SetLayoutDirty();
            transform.hasChanged = false;
        }
    }

    #endregion

    #region Layout

    public virtual void CalculateLayoutInputHorizontal() { }

    public virtual void CalculateLayoutInputVertical() { }

    public virtual void SetLayoutHorizontal() => CheckGraphicsDirty();

    public virtual void SetLayoutVertical() => CheckGraphicsDirty();

    public virtual float minWidth => -1;

    public virtual float preferredWidth
    {
        get
        {
            CheckGraphicsDirty();
            return orchestrator.outputNativeCanvasSize.x;
        }
    }

    public virtual float flexibleWidth => -1;

    public virtual float minHeight => -1;

    public virtual float preferredHeight
    {
        get
        {
            CheckGraphicsDirty();
            return orchestrator.outputNativeCanvasSize.y;
        }
    }

    public virtual float flexibleHeight => -1;

    public virtual int layoutPriority => -1;

    #endregion

}
