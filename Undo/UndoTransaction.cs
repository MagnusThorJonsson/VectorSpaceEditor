using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.Undo
{

    /// <summary>
    /// This acts as a container for multiple undo/redo records.
    /// </summary>
    public class UndoTransaction : IDisposable, IUndoRedoRecord
    {
        #region Variables & Properties
        private string _name;
        private List<IUndoRedoRecord> _undoRedoOperations = new List<IUndoRedoRecord>();
        
        /// <summary>
        /// The transaction name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The number of operations in this transaction
        /// </summary>
        public int OperationsCount
        {
            get { return _undoRedoOperations.Count; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a UndoTransaction
        /// </summary>
        /// <param name="name">The name of the action</param>
        public UndoTransaction(string name = "")
        {
            _name = name;
            UndoRedoManager.Instance().StartTransaction(this);
        }

        /// <summary>
        /// Disposes of this object
        /// </summary>
        public void Dispose()
        {
            UndoRedoManager.Instance().EndTransaction(this);
        }
        #endregion

        #region Transaction Methods
        /// <summary>
        /// Adds an operation to this transaction
        /// </summary>
        /// <param name="operation">The operation to add</param>
        public void AddUndoRedoOperation(IUndoRedoRecord operation)
        {
            _undoRedoOperations.Insert(0, operation);
        }

        /// <summary>
        /// Executes the action
        /// </summary>
        public void Execute()
        {
            _undoRedoOperations.ForEach((a) => a.Execute());
        }
        #endregion
    }
}
