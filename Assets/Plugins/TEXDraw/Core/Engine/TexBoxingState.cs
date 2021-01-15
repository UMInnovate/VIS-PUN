using System.Collections.Generic;
using UnityEngine;

namespace TexDrawLib
{
    public class TexBoxingState
    {
        public float width, height;
        public bool restricted, interned;

        readonly struct BoxingState {

            /// <summary> Available width </summary>
            public readonly float width;
            /// <summary> Available height </summary>
            public readonly float height;

            /// <summary> Is width is auto? </summary>
            public readonly bool restricted;
            /// <summary> Is height is auto? </summary>
            public readonly bool interned;

            public BoxingState(float width, float height, bool restricted, bool interned)
            {
                this.width = width;
                this.height = height;
                this.restricted = restricted;
                this.interned = interned;
            }
        }

        private readonly Stack<BoxingState> states = new Stack<BoxingState>();

        public void Push()
        {
            states.Push(new BoxingState(width, height, restricted, interned));
        }

        public void Pop()
        {
            var state = states.Pop();
            width = state.width;
            height = state.height;
            restricted = state.restricted;
            interned = state.interned;
        }

        public void Reset(Vector2 size)
        {
            states.Clear();
            width = size.x;
            height = size.y;
            restricted = false;
            interned = false;
        }
    }
}
