using System;
using System.Collections.Generic;
using UnityEngine;

namespace TexDrawLib
{
    public class TexRendererState
    {

        public readonly struct QuadState
        {
            public readonly int font;
            public readonly Rect vertex;
            public readonly Rect uv;
            public readonly Color32 color;

            public QuadState(int font, Rect vertex, Rect uv, Color color)
            {
                this.font = font;
                this.vertex = vertex;
                this.uv = uv;
                this.color = color;
            }
        }

        public readonly struct FlexibleUVQuadState
        {
            public readonly int font;
            public readonly Rect vertex;
            public readonly Vector2 uvBottomLeft, uvBottomRight, uvTopRight, uvTopLeft;
            public readonly Color32 color;

            public FlexibleUVQuadState(int font, Rect vertex, Vector2 uvBottomLeft, Vector2 uvBottomRight, Vector2 uvTopRight, Vector2 uvTopLeft, Color32 color)
            {
                this.font = font;
                this.vertex = vertex;
                this.uvBottomLeft = uvBottomLeft;
                this.uvBottomRight = uvBottomRight;
                this.uvTopRight = uvTopRight;
                this.uvTopLeft = uvTopLeft;
                this.color = color;
            }

            public FlexibleUVQuadState(int font, Rect vertex, in CharacterInfo info, Color32 color)
            {
                this.font = font;
                this.vertex = vertex;
                this.uvBottomLeft = info.uvBottomLeft;
                this.uvBottomRight = info.uvBottomRight;
                this.uvTopRight = info.uvTopRight;
                this.uvTopLeft = info.uvTopLeft;
                this.color = color;
            }
        }

        public readonly struct RawVertexQuadState
        {
            public readonly int font;
            public readonly Color32 color;
            public readonly Vector3 vBottomLeft, vBottomRight, vTopRight, vTopLeft;
            public readonly Vector2 uvBottomLeft, uvBottomRight, uvTopRight, uvTopLeft;

            public RawVertexQuadState(int font, Color32 color,
                Vector3 vBottomLeft, Vector3 vBottomRight, Vector3 vTopRight, Vector3 vTopLeft)
            {
                this.font = font;
                this.color = color;
                this.vBottomLeft = vBottomLeft;
                this.vBottomRight = vBottomRight;
                this.vTopRight = vTopRight;
                this.vTopLeft = vTopLeft;
                this.uvBottomLeft = new Vector2();
                this.uvBottomRight = new Vector2();
                this.uvTopRight = new Vector2();
                this.uvTopLeft = new Vector2();
            }

            public RawVertexQuadState(int font, Color32 color,
                Vector3 vBottomLeft, Vector3 vBottomRight, Vector3 vTopRight, Vector3 vTopLeft,
                Vector2 uvBottomLeft, Vector2 uvBottomRight, Vector2 uvTopRight, Vector2 uvTopLeft)
            {
                this.font = font;
                this.color = color;
                this.vBottomLeft = vBottomLeft;
                this.vBottomRight = vBottomRight;
                this.vTopRight = vTopRight;
                this.vTopLeft = vTopLeft;
                this.uvBottomLeft = uvBottomLeft;
                this.uvBottomRight = uvBottomRight;
                this.uvTopRight = uvTopRight;
                this.uvTopLeft = uvTopLeft;
            }

            public Rect CalculateRect()
            {
                return Rect.MinMaxRect(
                    Math.Min(Math.Min(this.vBottomLeft.x, this.vBottomRight.x), Math.Min(this.vTopLeft.x, this.vTopRight.x)),
                    Math.Min(Math.Min(this.vBottomLeft.y, this.vBottomRight.y), Math.Min(this.vTopLeft.y, this.vTopRight.y)),
                    Math.Max(Math.Max(this.vBottomLeft.x, this.vBottomRight.x), Math.Max(this.vTopLeft.x, this.vTopRight.x)),
                    Math.Max(Math.Max(this.vBottomLeft.y, this.vBottomRight.y), Math.Max(this.vTopLeft.y, this.vTopRight.y))
                );
            }
        }

        public float signedCoeff; // Additional constant required by TMP

        public List<FillHelper> vertexes;

        public Dictionary<int, int> vertexKeyFonts = new Dictionary<int, int>();

        public List<(string key, Rect area)> vertexLinks = new List<(string, Rect)>();

        public List<Color> vertexLinksOutput = new List<Color>();

        public float x, y, scale;

        public bool clip = true; public Rect clipRect;

        private readonly Stack<Vector2> positionStack = new Stack<Vector2>();

        public TexRendererState()
        {
        }

        public void Push()
        {
            positionStack.Push(new Vector2(x, y));
        }

        public void Pop()
        {
            var pos = positionStack.Pop();
            x = pos.x;
            y = pos.y;
        }

        public void Initialize(Rect rectArea, Vector3 nativeSize, Vector2 alignment)
        {
            if (vertexes != null)
            {
                ListPool<FillHelper>.FlushAndRelease(vertexes);
            }
            vertexes = ListPool<FillHelper>.Get();
            vertexKeyFonts.Clear();
            positionStack.Clear();
            vertexLinks.Clear();


            float alignX = 0;
            float alignY = alignment.y;

            var offsetY = (
                ((nativeSize.y) * alignY) + //Make sure the drawing pivot affected with aligment
                (-(nativeSize.z) * (1 - alignY)) + //Make sure the drawing pivot affected with aligment
                ((rectArea.height) * (-alignY + 0.5f)) + //Make sure it stick on rect bound
                -(rectArea.center.y)); //Make sure we calculate it from center (inside) of Rect no matter transform pivot has

            var offsetX = (
                (nativeSize.x * alignX) + //Make sure the drawing pivot affected with aligment
                (rectArea.width * (-alignX + 0.5f)) + //Make sure it stick on rect bound
                -(rectArea.center.x)); //Make sure we calculate it from center (inside) of Rect no matter transform pivot has

            x = -offsetX;
            y = -offsetY;
        }

        public FillHelper GetVertexForFont(int font)
        {
            if (vertexKeyFonts.TryGetValue(font, out int index))
            {
                return vertexes[index];
            }
            else
            {
                var vertex = FillHelper.Get();
                vertex.m_Font = font;
                vertexKeyFonts[font] = vertexes.Count;
                vertexes.Add(vertex);
                return vertex;
            }
        }

        public void Draw(in QuadState quad)
        {
            if (clip && !clipRect.Overlaps(quad.vertex))
                return;

            Vector3 p = quad.vertex.position;
            var uv = quad.uv.position;
            var color = quad.color;
            var vertex = GetVertexForFont(quad.font);

            vertex.AddVert(p, color, uv);  //Top-Left
            p.x += quad.vertex.size.x;
            uv.x += quad.uv.width;
            vertex.AddVert(p, color, uv);  //Top-Right
            p.y += quad.vertex.size.y;
            uv.y += quad.uv.height;
            vertex.AddVert(p, color, uv); //Bottom-Right
            p.x -= quad.vertex.size.x;
            uv.x -= quad.uv.width;
            vertex.AddVert(p, color, uv); //Bottom-Left
#if TEXDRAW_TMP
            for (int i = 0; i < 4; i++)
                vertex.m_Uv1S.Add(new Vector2(0, signedCoeff));
#endif
            vertex.AddQuad();
        }

        public void Draw(in FlexibleUVQuadState quad)
        {
            if (clip && !clipRect.Overlaps(quad.vertex))
                return;

            Vector3 p = quad.vertex.position;
            var vertex = GetVertexForFont(quad.font);

            vertex.AddVert(p, quad.color, quad.uvBottomLeft);  //Top-Left
            p.x += quad.vertex.size.x;
            vertex.AddVert(p, quad.color, quad.uvBottomRight);  //Top-Right
            p.y += quad.vertex.size.y;
            vertex.AddVert(p, quad.color, quad.uvTopRight); //Bottom-Right
            p.x -= quad.vertex.size.x;
            vertex.AddVert(p, quad.color, quad.uvTopLeft); //Bottom-Left
#if TEXDRAW_TMP
            for (int i = 0; i < 4; i++)
                vertex.m_Uv1S.Add(new Vector2(0, signedCoeff));
#endif
            vertex.AddQuad();
        }

        public void Draw(in RawVertexQuadState quad)
        {
            if (clip && !clipRect.Overlaps(quad.CalculateRect()))
                return;

            var vertex = GetVertexForFont(quad.font);

            vertex.AddVert(quad.vBottomLeft, quad.color, quad.uvBottomLeft);  //Top-Left
            vertex.AddVert(quad.vBottomRight, quad.color, quad.uvBottomRight);  //Top-Right
            vertex.AddVert(quad.vTopRight, quad.color, quad.uvTopRight); //Bottom-Right
            vertex.AddVert(quad.vTopLeft, quad.color, quad.uvTopLeft); //Bottom-Left
#if TEXDRAW_TMP
            for (int i = 0; i < 4; i++)
                vertex.m_Uv1S.Add(new Vector2(0, signedCoeff));
#endif
            vertex.AddQuad();
        }

        public Color DrawLink(string key, Rect area)
        {
            Color output;
            if (vertexLinksOutput.Count <= vertexLinks.Count)
                vertexLinksOutput.Add(output = Color.white);
            else
                output = vertexLinksOutput[vertexLinks.Count];
            vertexLinks.Add((key, area));
            return output;
        }
    }
}
