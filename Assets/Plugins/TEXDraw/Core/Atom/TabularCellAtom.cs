using UnityEngine;

namespace TexDrawLib
{
    public class TabularCellAtom : BlockAtom
    {
        public (float border, Color32 color, float padding) top, left, bottom, right;
        public (int span, float alignment, float size) x = (1, 0, 0), y = (1, 0, 0);
        public Color32 background;

        Box cacheBox;
        Vector3 cacheMetric;

        public override Box CreateBox(TexBoxingState state)
        {
            var hbox = HorizontalBox.Get(cacheBox ?? StrutBox.Empty, cacheMetric.x, x.alignment);
            // Horizontal
            if (left.padding > 0)
                hbox.Add(0, StrutBox.Get(left.padding, 0, 0));
            if (right.padding > 0)
                hbox.Add(StrutBox.Get(right.padding, 0, 0));
            var vbox = VerticalBox.Get(hbox, cacheMetric.y, cacheMetric.z);
            if (top.padding > 0)
                vbox.Add(0, StrutBox.Get(0, top.padding, 0));
            if (bottom.padding > 0)
                vbox.Add(StrutBox.Get(0, bottom.padding, 0));
            
            if (top.border > 0)
            {
                vbox.Add(0, StrutBox.Get(0, -top.border, 0));
                vbox.Add(0, RuleBox.Get(top.color, vbox.width, top.border, 0));
            }
            if (bottom.border > 0)
            {
                vbox.Add(StrutBox.Get(0, -bottom.border, 0));
                vbox.Add(RuleBox.Get(bottom.color, vbox.width, bottom.border, 0));
            }
            vbox.Normalize(cacheMetric.z + bottom.padding);
            var hhbox = HorizontalBox.Get(vbox);
            if (left.border > 0)
            {
                hhbox.Add(0, StrutBox.Get(-left.border, 0, 0));
                hhbox.Add(0, RuleBox.Get(left.color, left.border, vbox.height, vbox.depth));
            }
            if (right.border > 0)
            {
                hhbox.Add(StrutBox.Get(-right.border, 0, 0));
                hhbox.Add(RuleBox.Get(right.color, right.border, vbox.height, vbox.depth));
            }
            if (background.a > 0)
            {
                hhbox.Add(0, StrutBox.Get(-vbox.width, 0, 0));
                hhbox.Add(0, RuleBox.Get(background, vbox.width, vbox.height, vbox.depth));
            }
            return hhbox;
        }

        public void AssignFinalMetric(Vector3 size)
        {
            cacheMetric = size - excessSize;
        }

        public Vector3 PrecomputeBox(TexBoxingState state)
        {
            state.Push();
            if (x.size > float.Epsilon)
            {
                state.width = x.size;
                state.restricted = false;
            }
            else
                state.restricted = true;
            if (y.size > float.Epsilon)
            {
                state.height = y.size;
                state.interned = false;
            }
            else
                state.interned = true;
            cacheBox = atom?.CreateBox(state);
            state.Pop();
            return (cacheBox is null ? Vector3.zero : new Vector3(cacheBox.width, cacheBox.height, cacheBox.depth)) + excessSize;
        }

        Vector3 excessSize => new Vector3(
            left.padding + right.padding, 
            top.padding, bottom.padding);

        public override void Flush()
        {
            background.a = 0;
            top = left = bottom = right = default;
            x = y = (1, 0, 0);
            cacheBox = null;
            cacheMetric = Vector3.zero;
            ObjPool<TabularCellAtom>.Release(this);
            base.Flush();
        }

        internal static TabularCellAtom Get(Atom container)
        {
            var atom = ObjPool<TabularCellAtom>.Get();
            atom.atom = container;
            return atom;
        }

        internal static TabularCellAtom Get()
        {
            var atom = ObjPool<TabularCellAtom>.Get();
            return atom;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            if (state.Metadata.TryGetValue("columncolor", out Atom catom) && catom is ColorAtom ccatom)
            {
                background = ccatom.color;
            }
            top.padding = bottom.padding = left.padding = right.padding = state.Paragraph.lineSpacing;
            x.alignment = state.Paragraph.alignment;
        }
    }
}
