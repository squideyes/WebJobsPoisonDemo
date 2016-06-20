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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobsPoisonDemo.Demo.Workers
{
    public class GrayscaleImageWorker : WorkerBase<GrayscaleImageWork>
    {
        protected override async Task HandleWork(CloudStorageAccount account,
            CancellationToken token, GrayscaleImageWork work)
        {
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(
                WellKnown.ContainerNames.Observatories);

            var source = container.GetBlockBlobReference(work.Source);

            var oldBitmap = (Bitmap)Image.FromStream(await source.GetStreamAsync());

            var newBitmap = new Bitmap(oldBitmap.Width, oldBitmap.Height);

            var g = Graphics.FromImage(newBitmap);

            var colorMatrix = new ColorMatrix(
               new float[][]
               {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
               });

            var attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(oldBitmap, new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height),
                0, 0, oldBitmap.Width, oldBitmap.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();

            var target = container.GetBlockBlobReference(work.Target);

            await target.UploadBitmap(newBitmap, work.Format);
        }

        protected override string GetSuccessLogMessage(GrayscaleImageWork work) =>
            $"Converted \"{work.Source}\" to Grayscale (Saved To: \"{work.Target}\")";
    }
}
