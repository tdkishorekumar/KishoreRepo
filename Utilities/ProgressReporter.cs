using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.Utilities
{
    public interface IProgressReporter
    {
        void Initialize(int total);
        void Advance();
    }

    public sealed class ConsoleProgressReporter : IProgressReporter, IDisposable
    {
        private readonly string _Caption;
        private readonly object _Locker = new object();

        private int _Total;
        private int _Current;
        private bool _Initialized;
        private bool _Disposed;

        public ConsoleProgressReporter(string caption)
        {
            _Caption = caption;
        }

        public void Initialize(int total)
        {
            lock (_Locker)
            {
                if (_Disposed)
                    throw new ObjectDisposedException("ConsoleProgressReporter");

                if (_Initialized)
                    throw new InvalidOperationException("ConsoleProgressReporter is already initialized.");

                if (total < 0)
                    throw new ArgumentException("Total must be less than or equal to zero.");

                _Initialized = true;
                _Total = total;
                _Current = 0;

                Console.Write("{0} {1}/{2}", _Caption, 0, _Total);
            }
        }

        public void Advance()
        {
            lock (_Locker)
            {
                if (_Disposed)
                    throw new ObjectDisposedException("ConsoleProgressReporter");

                if (!_Initialized)
                    throw new InvalidOperationException("ConsoleProgressReporter is not initialized.");

                _Current++;

                if(_Current <= _Total)
                    Console.Write("\r{0} {1}/{2}", _Caption, _Current, _Total);
            }
        }

        public void Dispose()
        {
            lock (_Locker)
            {
                if (!_Disposed)
                {
                    _Disposed = true;
                    Console.WriteLine();
                }
            }
        }
    }
}
