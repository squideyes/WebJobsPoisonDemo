#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public abstract class WorkerBase<T> where T : WorkBase
    {
        private int maxTries;

        public WorkerBase(int maxTries = 1)
        {
            if (maxTries < 1)
                throw new ArgumentOutOfRangeException(nameof(maxTries));

            this.maxTries = maxTries;
        }

        public async Task ParseAndDoWork(string json, string queueName,
            int dequeueCount, CloudStorageAccount account, TextWriter log,
            CancellationToken token)
        {
            if (!queueName.IsQueueName())
                throw new ArgumentOutOfRangeException(nameof(queueName));

            if (dequeueCount < 1)
                throw new ArgumentOutOfRangeException(nameof(dequeueCount));

            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (token == null)
                throw new ArgumentNullException(nameof(token));

            try
            {
                var work = JsonConvert.DeserializeObject<T>(json);

                work.Validate();

                await HandleWork(account, token, work);

                await log.WriteLineAsync(GetSuccessLogMessage(work));
            }
            catch (JsonReaderException error)
            {
                if (!await PoisonInfo.Post<T>(account, queueName, json, error))
                    throw;
            }
            catch (ValidationException error)
            {
                if (!await PoisonInfo.Post<T>(account, queueName, json, error))
                    throw;
            }
            catch (Exception error)
            {
                if (dequeueCount < maxTries)
                    throw;

                if (!await PoisonInfo.Post<T>(account, queueName, json, error))
                    throw;
            }
        }

        protected abstract Task HandleWork(
            CloudStorageAccount account, CancellationToken token, T work);

        protected abstract string GetSuccessLogMessage(T work);
    }
}
