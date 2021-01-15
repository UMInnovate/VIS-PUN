using System;
using System.Collections.Generic;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public sealed class ParagraphAtom : LayoutAtom
    {

        float indent = 0;

        float alignment = 0;

        bool justified = false;

        float minHeight, minDepth, lineSpace, paraSpace, leftMargin, rightMargin;

        public static ParagraphAtom Get()
        {
            return ObjPool<ParagraphAtom>.Get();
        }

        public override void Flush()
        {
            ObjPool<ParagraphAtom>.Release(this);
            alignment = 0;
            indent = 0;
            justified = false;
            minHeight = 0;
            minDepth = 0;
            leftMargin = 0;
            rightMargin = 0;
            lineSpace = 0;
            paraSpace = 0;
            base.Flush();
            charBuildingBlock.Clear();
        }

        public readonly List<CharAtom> charBuildingBlock = new List<CharAtom>();

        public override void Add(Atom atom)
        {
            if (atom is CharAtom)
            {
                charBuildingBlock.Add((CharAtom)atom);
            }
            else
            {
                CleanupWord();
                base.Add(atom);
            }
        }

        public override Box CreateBox(TexBoxingState state)
        {
            CleanupWord();

            if (children.Count == 0)
            {
                var box = StrutBox.Get(0, 0, 0);
                return CheckBox(box, true);
            }

            state.Push();
            state.width -= leftMargin + rightMargin;
            state.interned = true;

            Box result = state.restricted ? CreateBoxRestricted(state) : CreateBoxWrappable(state);

            state.Pop();
            return CheckMaster(result);
        }

        public void CleanupWord()
        {
            if (charBuildingBlock.Count > 0)
            {
                var word = WordAtom.Get();
                word.Add(charBuildingBlock);
                charBuildingBlock.Clear();
                word.ProcessLigature();
                base.Add(word);
            }
        }

        private Box CreateBoxWrappable(TexBoxingState state)
        {
            var masterBox = VerticalBox.Get();
            Box overflowedBox = null;
            int i = 0;
            float oriStateWidth = state.width;
            while (i < children.Count || overflowedBox != null)
            {
                HorizontalBox box = HorizontalBox.Get();
                if (indent > 0)
                {
                    state.width = i == 0 ? oriStateWidth - indent : oriStateWidth;
                }
                if (overflowedBox != null)
                {
                    if (overflowedBox.width > state.width && children[i - 1] is WordAtom)
                        UnpackWordAndDoLetterWrapping(state, masterBox, ref box, overflowedBox);
                    else
                        box.Add(overflowedBox);
                    overflowedBox = null;
                }
                bool softlyBreak = false;
                for (; i < children.Count; i++)
                {
                    var draftBox = children[i].CreateBox(state);
                    if (box.children.Count == 0 && draftBox.width > state.width && children[i] is WordAtom)
                    {
                        // Need to unpack and do letter wrapping self
                        UnpackWordAndDoLetterWrapping(state, masterBox, ref box, draftBox);
                    }
                    else if (children[i] is ISoftBreak && i > 0)
                    {
                        softlyBreak = true;
                        overflowedBox = draftBox;
                        i++;
                        break;
                    }
                    else if ((box.width + draftBox.width > state.width && !(children[i] is SpaceAtom)))
                    {
                        overflowedBox = draftBox;
                        i++;
                        break;
                    }
                    else
                        box.Add(draftBox);
                }

                if (!state.restricted)
                {
                    if (FlexibleAtom.HandleFlexiblesHorizontal(box, state.width))
                    {
                        // Already justified.
                    }
                    else if (justified && !(i == children.Count && overflowedBox == null) && !softlyBreak)
                    {
                        JustifySpaces(state, box);
                    }
                    else if (alignment > 0)
                    {
                        var spaceAvailable = state.width - box.width;
                        var l = StrutBox.Get(spaceAvailable * alignment, 0, 0);
                        box.Add(0, l);
                    }


                    if (i == children.Count && overflowedBox == null)
                    {
                        if (masterBox.children.Count == 0)
                        {
                            masterBox.Flush();
                            return CheckBox(box, true);
                        }
                        else
                        {
                            masterBox.Add(CheckBox(box));
                        }
                    }
                    else
                    {
                        masterBox.Add(CheckBox(box, masterBox.children.Count == 0));
                    }
                } 
                else
                {
                    masterBox.Add(box);
                }

            }

            return masterBox;
        }

        private static void JustifySpaces(TexBoxingState state, HorizontalBox box)
        {
            var spaceAvailable = state.width - box.width;
            float spaceLastWidth = 0;
            int spaceCount = 0;
            for (int j = 1; j < box.children.Count; j++)
            {
                if (box.children[j] is StrutBox)
                {
                    spaceCount++;
                    spaceLastWidth = box.children[j].width;
                }
            }

            if (spaceCount > 1)
            {
                float unitSpace = (spaceAvailable + spaceLastWidth) / (spaceCount - 1);
                for (int j = 1; j < box.children.Count; j++)
                {
                    if (box.children[j] is StrutBox boxj)
                    {
                        boxj.width += unitSpace;
                    }
                }
            }

            box.Recalculate();
        }

        Box CheckBox(Box b, bool useindent = false)
        {
            b.height = Math.Max(b.height, minHeight) + lineSpace / 2;
            b.depth = Math.Max(b.depth, minDepth) + lineSpace / 2;
            if (useindent && indent > 0)
            {
                ((HorizontalBox)b).Add(0, StrutBox.Get(indent, 0, 0));
            }
            return b;
        }

        Box CheckMaster(Box b)
        {
            if (b is VerticalBox bb)
            {
                bb.children[0].height += paraSpace / 2;
            }
            b.height += paraSpace / 2;
            b.depth += paraSpace / 2;
            if (leftMargin == 0) return b;
            return HorizontalBox.Get(b, b.width + leftMargin, TexAlignment.Right);
        }

        Box CreateBoxRestricted(TexBoxingState state)
        {
            var box = HorizontalBox.Get();
            for (int i = 0; i < children.Count; i++)
            {
                box.Add(children[i].CreateBox(state));
            }
            return CheckBox(box);
        }

        private static void UnpackWordAndDoLetterWrapping(TexBoxingState state, VerticalBox masterBox, ref HorizontalBox box, Box draftBox)
        {
            var boxes = ((HorizontalBox)draftBox).children;
            for (int k = 0; k < boxes.Count; k++)
            {
                var box2 = boxes[k];
                if (box.width + box2.width > state.width)
                {
                    masterBox.Add(box);
                    box = HorizontalBox.Get();
                }
                box.Add(box2);
            }
            boxes.Clear();
            draftBox.Flush();
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            if (!state.Environment.current.IsInline())
            {
                var r = state.Ratio;
                alignment = state.Paragraph.alignment;
                indent = state.Paragraph.indent ? state.Paragraph.indentSpacing * r : 0;
                justified = state.Paragraph.justify;
                minHeight = state.Typeface.lineAscent * r;
                minDepth = state.Typeface.lineDescent * r;
                leftMargin = state.Paragraph.leftPadding * r;
                rightMargin = state.Paragraph.rightPadding * r;
                lineSpace = state.Paragraph.lineSpacing * r;
                paraSpace = state.Paragraph.paragraphSpacing * r;
            }

            if (value != null)
                SkipWhiteSpace(value, ref position);
        }

        public override bool HasContent()
        {
            return children.Count > 0 || charBuildingBlock.Count > 0;
        }
    }
}
