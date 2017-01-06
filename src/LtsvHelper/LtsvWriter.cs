﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;

namespace LtsvHelper
{
    /// <summary>
    /// Used to write LTSV files.
    /// </summary>
    public class LtsvWriter : ILtsvWriter
    {
        readonly LtsvSerializer _serializer;
        readonly List<KeyValuePair<string, string>> _currentRecord;

        public LtsvWriter(TextWriter textWriter)
        {
            _serializer = new LtsvSerializer(textWriter);
            _currentRecord = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Writes the field to the LTSV file.
        /// </summary>
        /// <param name="label">The label of field</param>
        /// <param name="value">The value of field</param>
        public void WriteField(string label, string value)
        {
            WriteField<string>(label, value);
        }

        /// <summary>
        /// Writes the field to the LTSV file.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="label">The label of field</param>
        /// <param name="value">The value of field</param>
        public void WriteField<T>(string label, T value)
        {
            _currentRecord.Add(new KeyValuePair<string, string>(label, value.ToString()));
        }

        /// <summary>
        /// Ends writing of the current record and starts a new record.
        /// This needs to be called to serialize the row to the writer.
        /// </summary>
        public void NextRecord()
        {
            _serializer.Write(_currentRecord);
            _currentRecord.Clear();
        }

        /// <summary>
        /// Writes the record to the LTSV file.
        /// </summary>
        /// <typeparam name="T">The type of the record.</typeparam>
        /// <param name="record">The record to write.</param>
        public void WriteRecord<T>(T record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            var properties = record.GetType()
                .GetRuntimeProperties()
                .Where(p => p.CanRead);
            foreach (var p in properties)
            {
                _currentRecord.Add(new KeyValuePair<string, string>(p.Name, p.GetValue(record).ToString()));
            }
            NextRecord();
        }

        #region IDisposable Support
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _serializer.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}