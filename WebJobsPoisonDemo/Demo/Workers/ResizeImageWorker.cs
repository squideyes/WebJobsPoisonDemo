#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.WindowsAzure.Storage;
using WebJobsPoisonDemo.CloudHelpers;
using WebJobsPoisonDemo.Demo.Shared;
using WebJobsPoisonDemo.Demo.Work;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobsPoisonDemo.Demo.Workers
{
    public class ResizeImageWorker : WorkerBase<ResizeImageWork>
    {
        protected override async Task HandleWork(CloudStorageAccount account,
            CancellationToken token, ResizeImageWork work)
        {
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(
                WellKnown.ContainerNames.Observatories);

            var source = container.GetBlockBlobReference(work.Source);

            var oldBitmap = (Bitmap)Image.FromStream(await source.GetStreamAsync());

            var newBitmap = (Bitmap)oldBitmap.GetThumbnailImage(
                work.Width, work.Height, null, new IntPtr());

            var target = container.GetBlockBlobReference(work.Target);

            await target.UploadBitmap(newBitmap, work.Format);
        }

        protected override string GetSuccessLogMessage(ResizeImageWork work) =>
            $"Resized \"{work.Source}\" (New Size: {work.Width}x{work.Height}, Saved To: \"{work.Target}\")";
    }
}
