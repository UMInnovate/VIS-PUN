using System;
using System.Collections.Generic;

using UnityEngine;

namespace TexDrawLib
{
    // Box containing vertical stack of child boxes.
    public class VerticalBox : LayoutBox
    {
        private float leftMostPos = float.MaxValue;
        private float rightMostPos = float.MinValue;

        public static VerticalBox Get(Box Box, float Height, TexAlignment Alignment)
        {
            var box = Get();
            if (Box.TotalHeight >= Height)
            {
                box.Add(Box);
                return box;
            }
            float rest = Height - Box.TotalHeight;
            if (Alignment == TexAlignment.Center)
            {
                var strutBox = StrutBox.Get(0, rest * 0.5f, 0);
                box.Add(strutBox);
                box.Add(Box);
                box.Add(strutBox);
                box.Shift(Box.height);
            }
            else if (Alignment == TexAlignment.Top)
            {
                box.Add(Box);
                box.Add(StrutBox.Get(0, rest, 0));
            }
            else if (Alignment == TexAlignment.Bottom)
            {
                box.Add(StrutBox.Get(0, rest, 0));
                box.Add(Box);
                box.Shift(-rest + Box.height);
            }
            return box;
        }

        public static VerticalBox Get(Box Box)
        {
            var box = Get();
            box.Add(Box);
            return box;
        }

        public static VerticalBox Get()
        {
            var box = ObjPool<VerticalBox>.Get();
            if (box.children == null)
                box.children = new List<Box>();
            return box;
        }

        //public static VerticalBox Get(Box box, float height, float alignment)
        //{
        //    var Box = Get();
        //    if (box.TotalHeight >= height)
        //    {
        //        Box.Add(box);
        //        return Box;
        //    }
        //    else
        //    {
        //        var excess = Math.Max(height - Box.TotalHeight, 0);
        //        Box.Add(StrutBox.Get(0, excess * alignment, 0));
        //        Box.Add(box);
        //        Box.Add(StrutBox.Get(0, excess * (1 - alignment), 0));
        //        Box.height = box.height + excess * alignment;
        //        Box.depth = box.depth + excess * (1 - alignment);
        //        Box.Shift(Box.depth * alignment);
        //    }
        //    return Box;
        //}

        public static VerticalBox Get(Box box, float minheight, float mindepth)
        {
            var Box = Get();
            var excessH = Math.Max(minheight - box.height, 0);
            var excessD = Math.Max(mindepth - box.depth, 0);
            Box.Add(StrutBox.Get(0, excessH, 0));
            Box.Add(box);
            Box.Add(StrutBox.Get(0, excessD, 0));
            Box.Normalize(box.depth + excessD);
            
            return Box;
        }

        public void Add(Box box)
        {
            children.Add(box);

            if (children.Count == 1)
            {
                height = box.height;
                depth = box.depth;
            }
            else
            {
                depth += box.height + box.depth;
            }
            RecalculateWidth(box);
        }

        public void Add(int position, Box box)
        {
            children.Insert(position, box);

            if (position == 0)
            {
                depth += box.depth + height;
                height = box.height;
            }
            else
            {
                depth += box.height + box.depth;
            }
            RecalculateWidth(box);
        }

        private void RecalculateWidth(Box box)
        {
            leftMostPos = Mathf.Min(leftMostPos, box.shift);
            rightMostPos = Mathf.Max(rightMostPos, box.shift + (box.width > 0 ? box.width : 0));
            width = rightMostPos;
        }

        public void Shift(float upward)
        {
            height += upward;
            depth -= upward;
        }

        public void Normalize(float newdepth)
        {
            height += depth - newdepth;
            depth = newdepth;
        }

        public override void Draw(TexRendererState state)
        {
            base.Draw(state);

            state.Push();

            state.y += height;
            var count = children.Count;

            for (int i = 0; i < count; i++)
            {
                Box child = children[i];
                state.y -= child.height;
                state.x += child.shift;
                child.Draw(state);
                state.x -= child.shift;
                state.y -= child.depth;
            }

            state.Pop();
        }

        public override void Flush()
        {
            ObjPool<VerticalBox>.Release(this);
           
            leftMostPos = float.MaxValue;
            rightMostPos = float.MinValue;

            base.Flush();
        }
    }
}
