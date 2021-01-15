using System.Collections.Generic;

namespace TexDrawLib
{
    public abstract class LayoutAtom : Atom
    {
        public List<Atom> children = new List<Atom>();

        public override CharType Type => CharTypeInternal.Inner;

        public virtual void Add(Atom atom)
        {
            children.Add(atom);
        }

        public override void Flush()
        {

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Flush();
            }
            children.Clear();
        }

        public virtual bool HasContent()
        {
            return children.Count > 0;
        }
    }
}
