using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.Undo
{
    /// <summary>
    /// Extension methods which allow a List to be used as a stack. This was created as we need to be able to manipulate the stack size dynamically which is not allowed by the Stack class
    /// </summary>
    public static class ListStackExtensions
    {
        /// <summary>
        /// Pushes an item into a stack
        /// </summary>
        /// <param name="list">The stack to push into</param>
        /// <param name="item">The item to push</param>
        public static void Push(this List<IUndoRedoRecord> list, IUndoRedoRecord item)
        {
            list.Insert(0, item);
        }

        /// <summary>
        /// Pops an item off a stack
        /// </summary>
        /// <param name="list">The stack to pop off of</param>
        /// <returns>The item that was popped off</returns>
        public static IUndoRedoRecord Pop(this  List<IUndoRedoRecord> list)
        {
            IUndoRedoRecord ret = list[0];
            list.RemoveAt(0);
            return ret;
        }
    }
}
