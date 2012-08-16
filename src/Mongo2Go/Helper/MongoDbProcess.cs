﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mongo2Go.Helper
{
    public partial class MongoDbProcess : IMongoDbProcess, IDisposable
    {
        private const string ProcessReadyIdentifier = "waiting for connections";
        private WrappedProcess _process;

        public IEnumerable<string> ErrorOutput { get; set; }
        public IEnumerable<string> StandardOutput { get; set; }

        internal MongoDbProcess(WrappedProcess process)
        {
            _process = process;
        }

        public IMongoDbProcess Start(string binariesDirectory, string dataDirectory, int port)
        {
            return Start(binariesDirectory, dataDirectory, port, false);
        }

        public IMongoDbProcess Start(string binariesDirectory, string dataDirectory, int port, bool doNotKill)
        {
            string fileName  = @"{0}\{1}".Formatted(binariesDirectory, MongoDbDefaults.MongodExecutable);
            string arguments = @"--dbpath ""{0}"" --port {1} --nohttpinterface --nojournal".Formatted(dataDirectory, port);

            WrappedProcess process = ProcessControl.ProcessFactory(fileName, arguments);
            process.DoNotKill = doNotKill;

            string windowTitle = "mongod | port: {0}".Formatted(port);
            ProcessOutput output = ProcessControl.StartAndWaitForReady(process, 5, ProcessReadyIdentifier, windowTitle);
            ErrorOutput = output.ErrorOutput;
            StandardOutput = output.StandardOutput;

            return new MongoDbProcess(process);
        }
    }
}
