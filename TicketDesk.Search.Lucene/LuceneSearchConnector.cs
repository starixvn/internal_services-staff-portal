using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.IO;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace TicketDesk.Search.Lucene
{
    public abstract class LuceneSearchConnector : IDisposable
    {
        private readonly object _getWriterLock = new object();
        private readonly object _getDirectoryLock = new object();
        private readonly object _getAnalyzerLock = new object();

        private Directory _tdIndexDirectory;
        private Analyzer _tdIndexAnalyzer;
        private IndexWriter _tdIndexWriter;

        protected LuceneSearchConnector(string indexLocation)
        {
            var fi = new DirectoryInfo(indexLocation);
            if (!fi.Exists)
            {
                var datadir = AppDomain.CurrentDomain.GetData("DataDirectory");
                IndexLocation = datadir == null ? "ram" : Path.Combine(datadir.ToString(), indexLocation);
            }
            else
            {
                IndexLocation = indexLocation;
            }
        }

        protected string IndexLocation { get; }

        public IndexWriter TdIndexWriter
        {
            get
            {
                if (_tdIndexWriter == null)
                {
                    InitWriter();
                }

                return _tdIndexWriter;
            }
        }

        protected Directory TdIndexDirectory
        {
            get
            {
                lock (_getDirectoryLock)
                {
                    if (_tdIndexDirectory == null)
                    {
                        if (string.Equals(IndexLocation, "ram", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _tdIndexDirectory = new RAMDirectory();
                        }
                        else
                        {
                            var dirInfo = new DirectoryInfo(IndexLocation);
                            _tdIndexDirectory = FSDirectory.Open(dirInfo);
                        }
                    }

                    return _tdIndexDirectory;
                }
            }
        }

        protected Analyzer TdIndexAnalyzer
        {
            get
            {
                lock (_getAnalyzerLock)
                {
                    return _tdIndexAnalyzer ?? (_tdIndexAnalyzer = new StandardAnalyzer(Version.LUCENE_30));
                }
            }
        }

        public void InitWriter()
        {
            lock (_getWriterLock)
            {
                if (IndexWriter.IsLocked(TdIndexDirectory))
                {
                    // directory locked, blow away the lock and hope there isn't
                    // really another writer open elsewhere
                    IndexWriter.Unlock(TdIndexDirectory);
                }

                _tdIndexWriter = new IndexWriter(
                    TdIndexDirectory,
                    TdIndexAnalyzer,
                    IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        public void ShutDownWriter()
        {
            _tdIndexWriter.Optimize();
            _tdIndexWriter = null;
            _tdIndexDirectory = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tdIndexDirectory?.Dispose();
                _tdIndexAnalyzer?.Dispose();
                _tdIndexWriter?.Dispose();
                TdIndexAnalyzer?.Dispose();
                TdIndexDirectory?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}