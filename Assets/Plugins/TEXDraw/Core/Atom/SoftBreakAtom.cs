using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public interface ISoftBreak
    {
    }

    public class SoftBreakAtom : Atom, ISoftBreak
    {
        public float extraSpace;

        public static SoftBreakAtom Get()
        {
            return ObjPool<SoftBreakAtom>.Get();
        }

        public override Box CreateBox(TexBoxingState state)
        {
            return StrutBox.Empty;
        }

        public override void Flush()
        {
            extraSpace = 0;
            ObjPool<SoftBreakAtom>.Release(this);
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            SkipWhiteSpace(value, ref position);
            if (position < value.Length && value[position] == '[')
            {
                extraSpace = TexUtility.ParseUnit(ReadGroup(value, ref position, '[', ']'), state.Document.pixelsPerInch, state.Document.initialSize);
            }
        }
    }

    public class ParagraphBreakAtom : SoftBreakAtom
    {
        public static new ParagraphBreakAtom Get()
        {
            return ObjPool<ParagraphBreakAtom>.Get();
        }

        public override void Flush()
        {
            extraSpace = 0;
            ObjPool<ParagraphBreakAtom>.Release(this);
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            SkipWhiteSpace(value, ref position);
        }
    }

    public class VerticalSkipAtom : SoftBreakAtom
    {

        public static new VerticalSkipAtom Get()
        {
            return ObjPool<VerticalSkipAtom>.Get();
        }

        public override void Flush()
        {
            extraSpace = 0;
            ObjPool<VerticalSkipAtom>.Release(this);
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            SkipWhiteSpace(value, ref position);
            if (position < value.Length)
            {
                extraSpace = TexUtility.ParseUnit(value, ref position, state);
            }
        }
    }
}
