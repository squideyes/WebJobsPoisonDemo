#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx;
using WebJobsPoisonDemo.CloudHelpers;
using WebJobsPoisonDemo.Demo.Shared;
using WebJobsPoisonDemo.Demo.Work;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using static WebJobsPoisonDemo.Properties.Resources;

namespace WebJobsPoisonDemo
{
    class Program
    {
        static void Main()
        {
            var watcher = new WebJobsShutdownWatcher();

            if (watcher.Token.IsCancellationRequested)
                return;

            AsyncContext.Run(() => PrepStorage(watcher.Token));

            if (watcher.Token.IsCancellationRequested)
                return;

            var config = new JobHostConfiguration();

            config.Tracing.ConsoleLevel = TraceLevel.Verbose;

            config.UseCore();

            config.UseSendGrid();

            var host = new JobHost(config);

            if (!watcher.Token.IsCancellationRequested)
                host.RunAndBlock();
        }

        private static async Task PrepStorage(CancellationToken token)
        {
            await Console.Out.WriteLineAsync("Prepping Storage Queues and Containers");

            var connString = AmbientConnectionStringProvider.Instance
                .GetConnectionString(ConnectionStringNames.Storage);

            var account = CloudStorageAccount.Parse(connString);

            await CreateQueuesAsync(account, token,
                WellKnown.QueueNames.PoisonInfos,
                ConvertImageWork.QueueName,
                GrayscaleImageWork.QueueName,
                ResizeImageWork.QueueName);

            var blobClient = account.CreateCloudBlobClient();

            await CreateContainersAsync(blobClient, token,
                WellKnown.ContainerNames.PoisonInfos,
                WellKnown.ContainerNames.Observatories);

            await UploadImagesIfNotExistAsync(blobClient, token);
        }

        private static async Task CreateContainersAsync(CloudBlobClient client, 
            CancellationToken token, params string[] containerNames)
        {
            foreach (var containerName in containerNames)
            {
                if (token.IsCancellationRequested)
                    return;

                var container = client.GetContainerReference(containerName);

                await container.CreateIfNotExistsAsync(token);
            }
        }

        private static async Task UploadImagesIfNotExistAsync(CloudBlobClient client,
            CancellationToken token)
        {
            var container = client.GetContainerReference(
                WellKnown.ContainerNames.Observatories);

            await UploadIfNotExistsAsync(DavidDunlapObservatory,
                container, "Originals/DavidDunlapObservatory.jpg", token);

            await UploadIfNotExistsAsync(LaddObservatory,
                container, "Originals/LaddObservatory.jpg", token);

            await UploadIfNotExistsAsync(HobbyEberlyObservatory,
                container, "Originals/HobbyEberlyObservatory.jpg", token);
        }

        private static async Task CreateQueuesAsync(CloudStorageAccount account,
            CancellationToken token, params string[] queueNames)
        {
            var client = account.CreateCloudQueueClient();

            foreach (var queueName in queueNames)
            {
                if (token.IsCancellationRequested)
                    return;

                var queue = client.GetQueueReference(queueName);

                await queue.CreateIfNotExistsAsync(token);
            }
        }

        private static async Task UploadIfNotExistsAsync(Image image,
            CloudBlobContainer container, string blobName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var blob = container.GetBlockBlobReference(blobName);

            if (token.IsCancellationRequested || await blob.ExistsAsync(token))
                return;

            var bytes = image.ToArray();

            if (!token.IsCancellationRequested)
                await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, token);

            if (!token.IsCancellationRequested)
            {
                await Console.Out.WriteLineAsync(
                    $"Uploaded \"{blobName}\" to the \"{container.Name}\" container");
            }
        }
    }
}
