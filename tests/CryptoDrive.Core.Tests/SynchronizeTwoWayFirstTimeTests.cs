using CryptoDrive.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace CryptoDrive.Core.Tests
{
    public class SynchronizeTwoWayFirstTimeTests : IDisposable
    {
        private List<ILoggerProvider> _loggerProviders;
        private ILogger<CryptoDriveSyncEngine> _logger;

        private DriveHive _driveHive;

        public SynchronizeTwoWayFirstTimeTests(ITestOutputHelper xunitLogger)
        {
            // logger
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder
                .AddSeq()
                .AddProvider(new XunitLoggerProvider(xunitLogger))
                .SetMinimumLevel(LogLevel.Trace);

                _loggerProviders = loggingBuilder.Services
                    .Where(descriptor => typeof(ILoggerProvider).IsAssignableFrom(descriptor.ImplementationInstance?.GetType()))
                    .Select(descriptor => (ILoggerProvider)descriptor.ImplementationInstance)
                    .ToList();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetService<ILogger<CryptoDriveSyncEngine>>();
        }

        private async void Execute(string fileId, Action assertAction)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CryptoDriveDbContext>()
                // InMemoryDatabase is currently broken
                //.UseInMemoryDatabase(databaseName: "CryptoDrive")
                .UseSqlite($"Data Source={Path.GetTempFileName()}")
                .Options;

            _driveHive = await Utils.PrepareDrives(fileId, _logger);

            using (var context = new CryptoDriveDbContext(options))
            {
                context.Database.EnsureCreated();

                var synchronizer = new CryptoDriveSyncEngine(_driveHive.RemoteDrive, _driveHive.LocalDrive, context, _logger);

                // Act
                await synchronizer.Synchronize();

                // Assert
                assertAction?.Invoke();
            }
        }

        [Fact]
        public void CanSyncFirstTimeATest()
        {
            this.Execute("a", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("a".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("a"
                    .ToConflictFilePath(Utils.DriveItemPool["a2"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("a".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("a"
                    .ToConflictFilePath(Utils.DriveItemPool["a2"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["a2"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeBTest()
        {
            this.Execute("b", () =>
            {
                Assert.True(File.Exists("b".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("b".ToAbsolutePath(_driveHive.RemoteDrivePath)));
            });
        }

        [Fact]
        public void CanSyncFirstTimeCTest()
        {
            this.Execute("c", () =>
            {
                Assert.True(File.Exists("c".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("c".ToAbsolutePath(_driveHive.RemoteDrivePath)));
            });
        }

        [Fact]
        public void CanSyncFirstTimeDTest()
        {
            this.Execute("d", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("d".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("d".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("d".ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["d1"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeETest()
        {
            this.Execute("e", () =>
            {
                Assert.True(File.Exists("e"
                    .ToConflictFilePath(Utils.DriveItemPool["e1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("e".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("e".ToAbsolutePath(_driveHive.RemoteDrivePath)));
            });
        }

        [Fact]
        public void CanSyncFirstTimeFTest()
        {
            this.Execute("f", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("f"
                    .ToConflictFilePath(Utils.DriveItemPool["f1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("f".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("f".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("f".ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["f2"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeGTest()
        {
            this.Execute("g", () =>
            {
                Assert.True(File.Exists("g".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("g"
                    .ToConflictFilePath(Utils.DriveItemPool["g1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("g".ToAbsolutePath(_driveHive.RemoteDrivePath)));
            });
        }

        [Fact]
        public void CanSyncFirstTimeHTest()
        {
            this.Execute("h", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("h".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("h"
                    .ToConflictFilePath(Utils.DriveItemPool["h1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("h".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("h"
                    .ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["h1"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeITest()
        {
            this.Execute("i", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("i"
                    .ToConflictFilePath(Utils.DriveItemPool["i1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("i".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("i".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("i"
                    .ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["i2"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeJTest()
        {
            this.Execute("j", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("j"
                    .ToConflictFilePath(Utils.DriveItemPool["j1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("j".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("j"
                    .ToConflictFilePath(Utils.DriveItemPool["j3"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("j".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("j"
                    .ToConflictFilePath(Utils.DriveItemPool["j3"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["j3"].QuickXorHash());
                }
            });
        }

        [Fact]
        public void CanSyncFirstTimeKTest()
        {
            this.Execute("k", () =>
            {
                var hashAlgorithm = new QuickXorHash();

                Assert.True(File.Exists("k".ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("k"
                    .ToConflictFilePath(Utils.DriveItemPool["k1"].LastModified())
                    .ToAbsolutePath(_driveHive.LocalDrivePath)));
                Assert.True(File.Exists("k".ToAbsolutePath(_driveHive.RemoteDrivePath)));

                using (var stream = File.OpenRead("k".ToAbsolutePath(_driveHive.LocalDrivePath)))
                {
                    Assert.True(Convert.ToBase64String(hashAlgorithm.ComputeHash(stream)) == Utils.DriveItemPool["k1"].QuickXorHash());
                }
            });
        }

        public void Dispose()
        {
            _loggerProviders.ForEach(loggerProvider => loggerProvider.Dispose());

            Directory.Delete(_driveHive.RemoteDrivePath, true);
            Directory.Delete(_driveHive.LocalDrivePath, true);
        }
    }
}