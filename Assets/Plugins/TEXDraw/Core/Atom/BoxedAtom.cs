﻿using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public class BoxedAtom : Atom
    {
        public Atom atom;
        public bool horizontal;
        public float size;
        public bool relative = true;

        // public override Atom Unpack() => size == 0 && relative ? base.Unpack() : null;

        public static BoxedAtom Get()
        {
            return ObjPool<BoxedAtom>.Get();
        }

        public override Box CreateBox(TexBoxingState state)
        {
            if (horizontal)
            {
                var hbox = HorizontalBox.Get();
                state.Push();
                state.restricted = relative;
                if (size > 0)
                    state.width = size;
                hbox.Add(atom.CreateBox(state));
                hbox.width = System.Math.Max(hbox.width, (relative ? hbox.width : 0) + size);
                state.Pop();
                return hbox;
            }
            else
            {
                var vbox = VerticalBox.Get();
                state.Push();
                state.interned = relative;
                if (size > 0)
                    state.height = size;
                vbox.Add(atom.CreateBox(state));
                vbox.depth = System.Math.Max(vbox.TotalHeight, (relative ? vbox.TotalHeight : 0) + size) - vbox.height;
                state.Pop();
                return vbox;
            }
        }

        public override void Flush()
        {
            size = 0;
            relative = true;
            ObjPool<BoxedAtom>.Release(this);
            atom?.Flush();
            atom = null;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            horizontal = command != "vbox";
            SkipWhiteSpace(value, ref position);
            if (position < value.Length && value[position] != beginGroupChar)
            {
                var key = LookForAWord(value, ref position);
                if (key == "by" || key == "spread")
                {
                    relative = key == "spread";
                    SkipWhiteSpace(value, ref position);
                    size = TexUtility.ParseUnit(value, ref position, state);
                    SkipWhiteSpace(value, ref position);
                }
            }
            atom = state.parser.Parse(ReadGroup(value, ref position), state);
        }
    }
}
