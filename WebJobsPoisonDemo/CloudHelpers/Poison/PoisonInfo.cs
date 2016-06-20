#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using WebJobsPoisonDemo.Demo.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public class PoisonInfo
    {
        [Required]
        public Guid PoisonInfoId { get; set; }

        [Required]
        public string QueueName { get; set; }

        [Required]
        public string WorkAsJson { get; set; }

        [Required]
        public string WorkType { get; set; }

        [Required]
        public ErrorInfo ErrorInfo { get; set; }

        [Required]
        public DateTime DetectedOn { get; set; }

        public string BlobName
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("POISON_");
                sb.Append(DetectedOn.ToString("yyyyMMdd_HHmmssfff"));
                sb.Append('_');
                sb.Append(PoisonInfoId.ToString("N"));
                sb.Append(".json");

                return sb.ToString();
            }
        }

        public static async Task<bool> Post<T>(CloudStorageAccount account,
            string queueName, string workAsJson, Exception error)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (!queueName.IsQueueName())
                throw new ArgumentOutOfRangeException(nameof(queueName));

            if (error == null)
                throw new ArgumentNullException(nameof(error));

            var info = new PoisonInfo()
            {
                PoisonInfoId = Guid.NewGuid(),
                QueueName = queueName,
                WorkType = typeof(T).FullName,
                WorkAsJson = workAsJson,
                ErrorInfo = ErrorInfo.Create(error),
                DetectedOn = DateTime.UtcNow
            };

            var client = account.CreateCloudQueueClient();

            var queue = client.GetQueueReference(WellKnown.QueueNames.PoisonInfos);

            await queue.AddMessageAsync(
                new CloudQueueMessage(JsonConvert.SerializeObject(info)));

            return true;
        }
    }
}
