namespace TicketDesk.Domain.Legacy
{
    using Domain.Migrations;
    using IO;
    using Model;
    using System;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public delegate void MigratorStatusChangedHandler(string[] messages);

    public class LegacyMigrator
    {
        private bool _isRunning;

        public event MigratorStatusChangedHandler StatusChanged;

        public CancellationTokenSource Canceler { get; set; }

        public void CancelImport()
        {
            Canceler.Cancel();
        }

        public void Migrate()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("This migrator is already running an operation");
            }

            _isRunning = true;

            // if not supplied by caller, init our own cancel source
            if (Canceler == null)
            {
                Canceler = new CancellationTokenSource();
            }

            // setup run time stats
            var startImport = DateTime.Now;
            try
            {
                if (!Canceler.IsCancellationRequested)
                {
                    RaiseStatusChangedOnMainThread(new[] { "Beginning migration." });

                    MigrateImages();
                    MigrateDatabase();
                }
            }
            catch (OperationCanceledException)
            {
                // we'll eat the exceptions if thrown due to cancellation
            }
            catch (AggregateException ax)
            {
                foreach (Exception ex in ax.InnerExceptions)
                {
                    RaiseStatusChangedOnMainThread(new[] { ex.GetType().Name, ex.Message, ex.StackTrace });
                }

                throw;
            }
            catch (Exception ex)
            {
                RaiseStatusChangedOnMainThread(new[] { ex.GetType().Name, ex.Message, ex.StackTrace });
                if (ex.InnerException != null)
                {
                    RaiseStatusChangedOnMainThread(new[] { ex.InnerException.GetType().Name, ex.InnerException.Message, ex.InnerException.StackTrace });
                }

                throw;
            }
            finally
            {
                // if user canceled
                if (Canceler.IsCancellationRequested)
                {
                    RaiseStatusChangedOnMainThread(new[] { "Cancellation requested" });
                }

                // output run time stats
                var endImport = DateTime.Now;
                var ts = endImport - startImport;
                var time =
                    $"Total Execution Time: {Math.Truncate(ts.TotalHours):00} hr {ts.Minutes:00} mn {ts.Seconds:00} sec ";
                RaiseStatusChangedOnMainThread(new[] { time });
                _isRunning = false;
            }
        }

        private void MigrateDatabase()
        {
            RaiseStatusChangedOnMainThread(new[] { "Converting database." });

            using (var ctx = new TdDomainContext(null))
            {
                TdLegacyDatabaseInitializer<TdDomainContext>.InitDatabase(ctx);
            }

            RaiseStatusChangedOnMainThread(new[] { "Converion complete." });
            RaiseStatusChangedOnMainThread(new[] { "Migrating database to latest version." });

            using (var ctx = new TdDomainContext(null))
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<TdDomainContext, Configuration>(true));
                ctx.Database.Initialize(true);
            }

            RaiseStatusChangedOnMainThread(new[] { "Database migrated to latest version." });
        }

        private void MigrateImages()
        {
            const string contentQueryText = "SELECT FileContents FROM dbo.TicketAttachments WHERE FileId = @p0";
            const string modelQuery = "SELECT TicketId, FileId, FileName, FileSize, IsPending FROM dbo.TicketAttachments";
            const int pageSize = 20;

            using (var ctx = new TdDomainContext(null))
            {
                var q = ctx.Database.SqlQuery<TicketAttachment>(modelQuery);
                var numRec = q.Count();
                RaiseStatusChangedOnMainThread(new[] { $"Importing {numRec} Images from legacy database." });
                var i = 0;
                while (i < numRec)
                {
                    var res = q.Skip(i).Take(pageSize).ToArray();
                    i += pageSize;
                    foreach (var file in res)
                    {
                        if (file.TicketId.HasValue)
                        {
                            var container = file.TicketId.Value.ToString(CultureInfo.InvariantCulture);
                            if (!TicketDeskFileStore.FileExists(file.FileName, container, false))
                            {
                                var contentQuery = ctx.Database.SqlQuery<byte[]>(contentQueryText, file.FileId);
                                var stream = new MemoryStream(contentQuery.First());

                                TicketDeskFileStore.SaveAttachmentAsync(stream, file.FileName, container, false).Wait();
                                RaiseStatusChangedOnMainThread(new[] {
                                    $"    Image {file.FileName} for ticket #{file.TicketId} imported."
                                });
                            }
                            else
                            {
                                RaiseStatusChangedOnMainThread(new[] {
                                    $"    Image {file.FileName} for ticket #{file.TicketId} already exists, skipping."
                                });
                            }
                        }
                    }
                }

                RaiseStatusChangedOnMainThread(new[] { "Image import complete." });
            }
        }

        #region utility

        internal void RaiseStatusChangedOnMainThread(string[] messages)
        {
            RaiseEventOnMainThread(StatusChanged, messages);
        }

        private static void RaiseEventOnMainThread<TArgType>(Delegate theEvent, TArgType[] args) where TArgType : class
        {
            foreach (Delegate d in theEvent.GetInvocationList())
            {
                ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                if (syncer == null)
                {
                    d.DynamicInvoke(new[] { args });
                }
                else
                {
                    syncer.BeginInvoke(d, args);
                }
            }
        }

        #endregion utility
    }
}