using System.Collections.Generic;
using UnityEngine;
using static TexDrawLib.TexParserUtility;
using static TexDrawLib.StrikeBox;

//Atom for Creating Diagonal Negate Line
namespace TexDrawLib
{
    public class NegateAtom : Atom
    {
        public Atom atom;
        public StrikeMode mode = 0;

        public float offsetM = 0;
        public float offsetP = 0;
        public float thickness;
        public float framePadding;
        public float underlineLevel;
        public float overlineLevel;
        public float midlineLevel;
        public Color32 color;

        public override CharType Type => CharTypeInternal.Inner;

        public static NegateAtom Get()
        {
            return ObjPool<NegateAtom>.Get();
        }

        static public readonly Dictionary<string, StrikeMode> modes = new Dictionary<string, StrikeBox.StrikeMode>() {
            { "not", StrikeMode.diagonal },
            { "nnot", StrikeMode.diagonalInverse },
            { "hnot", StrikeMode.horizontal },
            { "dnot", StrikeMode.doubleHorizontal },
            { "unot", StrikeMode.underline },
            { "onot", StrikeMode.overline },
            { "vnot", StrikeMode.vertical },
            { "vnnot", StrikeMode.verticalInverse },
            { "underline", StrikeMode.underline },
            { "overline", StrikeMode.overline },
        };

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var r = state.Ratio;
            modes.TryGetValue(command, out this.mode);
            this.color = state.Color.current;
            this.thickness = state.Math.lineThinkness * r;
            this.underlineLevel = state.Typeface.underlineLevel * r;
            this.midlineLevel = state.Typeface.midlineLevel * r;
            this.overlineLevel = state.Typeface.overlineLevel * r;
            this.framePadding = command == "overline" || command == "underline" ? 0 : state.Math.framePadding * r;

            if (position < value.Length && value[position] == '[')
            {
                var offset = ReadGroup(value, ref position, '[', ']');
                int pos = offset.IndexOf('-');
                if (pos < 0 || !float.TryParse(offset.Substring(pos), out this.offsetM))
                    this.offsetM = 0;
                if (pos < 1 || !float.TryParse(offset.Substring(0, pos), out this.offsetP))
                {
                    if (pos == 0 || !float.TryParse(offset, out this.offsetP))
                        this.offsetP = 0;
                }
            }
            else
            {
                this.offsetM = 0;
                this.offsetP = 0;
            }
            this.atom = state.parser.ParseToken(value, state, ref position);

        }

        public override Box CreateBox(TexBoxingState state)
        {
            if (this.atom == null)
                return StrutBox.Empty;
            var baseBox = this.atom.CreateBox(state);
            var result = HorizontalBox.Get();

            var negateBox = StrikeBox.Get(color,
                                baseBox.height, baseBox.width, baseBox.depth,
                                this.framePadding, this.thickness / 2,
                                mode, offsetM, offsetP,
                                underlineLevel, overlineLevel, midlineLevel);

            negateBox.shift = baseBox.shift;
            result.Add(negateBox);
            result.Add(StrutBox.Get(-baseBox.width, 0, 0));
            result.Add(baseBox);
            return result;
        }

        public override void Flush()
        {
            ObjPool<NegateAtom>.Release(this);

            if (this.atom != null)
            {
                this.atom.Flush();
                this.atom = null;
            }
        }
    }
}
