using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.Undo
{
    /// <summary>
    /// This is implemented by classes which act as records for storage of undo/redo records
    /// </summary>
    public interface IUndoRedoRecord
    {
        /// <summary>
        /// The name of the record
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the record
        /// </summary>
        void Execute();
    }

}
