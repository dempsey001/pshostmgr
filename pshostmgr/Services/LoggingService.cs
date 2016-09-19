/*
 * MIT License
 * 
 * Copyright (c) 2016 Joseph Dempsey
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.IO;
using System.Diagnostics;

namespace ManageHosts.Services
{
#if DEBUG
	using DefaultLog = ConsoleLog;
#else
	using DefaultLog = NullLog;
#endif

	/// <summary>
	/// Provies a very simple interface to perform
	/// logging of application data.
	/// </summary>
	public interface ILoggingService
	{
		/// <summary>
		/// Used to write general info to the log.
		/// </summary>
		/// <param name="msg">Message to write</param>
		/// <param name="data">Format params</param>
		void WriteLog(string msg, params object[] data);

		/// <summary>
		/// Used to write error info to the log.
		/// </summary>
		/// <param name="msg">Message to write</param>
		/// <param name="data">Format params</param>
		void WriteError(string msg, params object[] data);

		/// <summary>
		/// Specific error relating to entity/record vaidation.
		/// </summary>
		/// <param name="msg">Message to write</param>
		/// <param name="data">Format params</param>
		void WriteValidationError(string msg, params object[] data);

		// END INTERFACE
	}

	/// <summary>
	/// Simple implementation of a ILoggingService that is
	/// basically a "&> /dev/null"
	/// </summary>
	internal sealed class NullLog : ILoggingService
	{
		/// <inheritdoc />
		public void WriteLog(string msg, params object[] data)
		{
			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteError(string msg, params object[] data)
		{
			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteValidationError(string msg, params object[] data)
		{
			// END FUNCTION
		}

		// END CLASS (NullLog)
	}

	/// <summary>
	/// Simple implementation of a ILoggingService that dumps
	/// all data to the console. This is the default log
	/// type if no log is set.
	/// </summary>
	internal sealed class ConsoleLog : ILoggingService
	{
		/// <inheritdoc />
		public void WriteLog(string msg, params object[] data)
		{
			var composite = String.Format(msg, data);
			Console.WriteLine(
				$"[{DateTime.Now} - INFO]: {composite}");

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteError(string msg, params object[] data)
		{
			var composite = String.Format(msg, data);
			Console.WriteLine(
				$"[{DateTime.Now} - ERROR]: {composite}");

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteValidationError(string msg, params object[] data)
		{
			var composite = String.Format(msg, data);
			Console.WriteLine(
				$"[{DateTime.Now} - VALIDATION]: {composite}");

			// END FUNCTION
		}

		// END CLASS (ConsoleLog)
	}

	/// <summary>
	///	Implements ILoggingService as system event logger.
	/// </summary>
	internal sealed class SysEventLog : ILoggingService
	{
		private const string TargetEventLog = "Application";
		private string _evSource;

		/// <summary>
		/// Notes where the event data is logged to
		/// </summary>
		public string EventSource
		{
			get { return _evSource; }
			set
			{
				_evSource = value;
				if (!EventLog.SourceExists(EventSource))
					EventLog.CreateEventSource(EventSource, TargetEventLog);
			}

			// END ACCESSOR (EventSource)
		}

		/// <summary>
		/// sets up a default bogus event source.
		/// </summary>
		public SysEventLog()
		{
			EventSource = "DEFAULT-183901";

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteLog(string msg, params object[] data)
		{
			EventLog.WriteEntry(
				EventSource,
				String.Format(msg, data),
				EventLogEntryType.Information);

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteError(string msg, params object[] data)
		{
			EventLog.WriteEntry(
				EventSource,
				String.Format(msg, data),
				EventLogEntryType.Error);

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteValidationError(string msg, params object[] data)
		{
			WriteError(msg, data);

			// END FUNCTION
		}

		// END CLASS (SysEventLog)
	}


	/// <summary>
	///	Implements ILoggingService as a to disk file log.
	/// </summary>
	internal sealed class FileLog : ILoggingService, IDisposable
	{
		private readonly StreamWriter _writer = null;

		/// <summary>
		/// Ctor. Used to setup the log to a path that the file
		/// data can write to.
		/// </summary>
		/// <param name="path">File path to write log to.</param>
		public FileLog(string path)
		{
			var fs = File.Open(
				path, // File to open
				FileMode.Append | FileMode.OpenOrCreate, // add to it if it exists, else create it.
				FileAccess.Write, // We only need to write to the log
				FileShare.Read ); // Allow other apps (notepad, etc..) to read the log file
			_writer = new StreamWriter( fs );

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteLog(string msg, params object[] data)
		{
			var composite = String.Format( msg, data );
			_writer.WriteLine(
				$"[{DateTime.Now} - INFO]: {composite}" );
			_writer.Flush();

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteError(string msg, params object[] data)
		{
			var composite = String.Format( msg, data );
			_writer.WriteLine(
				$"[{DateTime.Now} - ERROR]: {composite}" );
			_writer.Flush();

			// END FUNCTION
		}

		/// <inheritdoc />
		public void WriteValidationError(string msg, params object[] data)
		{
			var composite = String.Format( msg, data );
			_writer.WriteLine(
				$"[{DateTime.Now} - VALIDATION]: {composite}" );
			_writer.Flush();

			// END FUNCTION
		}

		// END CLASS (FileLog)

        /// <summary>
		/// This is the publicly accessible Dispose() impl required
		/// by the IDisposable interface. We dispose of our data and
		/// tell the finalizer not to run.
		/// </summary>
		public void Dispose()
		{
			_writer?.Dispose();
			GC.SuppressFinalize( this );

			// END FUNCTION
		}

		// END CLASS (FileLog)

	}

	// END NAMESPACE
}
