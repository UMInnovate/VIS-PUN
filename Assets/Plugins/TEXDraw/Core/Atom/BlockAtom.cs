
namespace TexDrawLib
{
    public abstract class BlockAtom : Atom
    {
        public Atom atom;

        public virtual Atom Unpack() => atom;

        public bool IsUnpackable => Unpack() != null;

        public override void Flush()
        {
            atom?.Flush();
            atom = null;
        }

        public override Box CreateBox(TexBoxingState state)
        {
            return atom?.CreateBox(state) ?? StrutBox.Get(0, 0, 0);
        }
    }
}
