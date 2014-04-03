using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.Undo
{
    /// <summary>
    /// The Undo Redo Operation delegate
    /// </summary>
    /// <typeparam name="T">The object</typeparam>
    /// <param name="undoData">The data</param>
    public delegate void UndoRedoOperation<T>(T undoData);

    /// <summary>
    /// Contains information about an undo or redo record
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoRedoRecord<T> : IUndoRedoRecord
    {
        #region Variables & Properties
        private UndoRedoOperation<T> _operation;
        private T _undoData;
        private string _description;

        /// <summary>
        /// The name of the record
        /// </summary>
        public string Name
        {
            get { return _description; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public UndoRedoRecord()
        {
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="operation">The operation</param>
        /// <param name="undoData">The data</param>
        /// <param name="description">The description</param>
        public UndoRedoRecord(UndoRedoOperation<T> operation, T undoData, string description = "")
        {
            SetInfo(operation, undoData, description);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the description on the record
        /// </summary>
        /// <param name="operation">The operation</param>
        /// <param name="undoData">The data</param>
        /// <param name="description">The description</param>
        public void SetInfo(UndoRedoOperation<T> operation, T undoData, string description = "")
        {
            _operation = operation;
            _undoData = undoData;
            _description = description;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        public void Execute()
        {
            //Trace.TraceInformation("Undo/redo operation {0} with data {1} - {2}", _operation, _undoData, _description);
            _operation(_undoData);
        }
        #endregion
    }

}
