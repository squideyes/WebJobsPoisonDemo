#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SendGrid;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebJobsPoisonDemo.CloudHelpers;
using WebJobsPoisonDemo.Demo.Shared;
using WebJobsPoisonDemo.Demo.Work;
using WebJobsPoisonDemo.Demo.Workers;

namespace WebJobsPoisonDemo
{
    public class Functions
    {
        public async Task HandlePoisonInfo(TextWriter log, [SendGrid] SendGridMessage message,
            [QueueTrigger(WellKnown.QueueNames.PoisonInfos)] PoisonInfo info,
            [Blob(WellKnown.ContainerNames.PoisonInfos + "/{BlobName}")] CloudBlockBlob blob)
        {
            await PoisonHandler.SaveAsBlobThenEmailAlerts(ConfigurationManager.AppSettings["alertFrom"],
               ConfigurationManager.AppSettings["alertTos"].Split(',').ToList(), log, message, info, blob);
        }

        public async Task ConvertImage([QueueTrigger(ConvertImageWork.QueueName)] string json,
            CloudStorageAccount account, TextWriter log, int dequeueCount, CancellationToken token)
        {
            await new ConvertImageWorker().ParseAndDoWork(
                json, ConvertImageWork.QueueName, dequeueCount, account, log, token);
        }

        public async Task GrayscaleImage([QueueTrigger(GrayscaleImageWork.QueueName)] string json,
            CloudStorageAccount account, TextWriter log, int dequeueCount, CancellationToken token)
        {
            await new GrayscaleImageWorker().ParseAndDoWork(
                json, GrayscaleImageWork.QueueName, dequeueCount, account, log, token);
        }

        public async Task ResizeImage([QueueTrigger(ResizeImageWork.QueueName)] string json,
            CloudStorageAccount account, TextWriter log, int dequeueCount, CancellationToken token)
        {
            await new ResizeImageWorker().ParseAndDoWork(
                json, ResizeImageWork.QueueName, dequeueCount, account, log, token);
        }
    }
}
