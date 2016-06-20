#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.WindowsAzure.Storage.Blob;
using WebJobsPoisonDemo.Demo.Shared;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SDI = System.Drawing.Imaging;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public static class CloudBlockBlobExtenders
    {
        public static async Task<Stream> GetStreamAsync(this CloudBlockBlob blob)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

            var stream = new MemoryStream();

            await blob.DownloadToStreamAsync(stream);

            stream.Position = 0;

            return stream;
        }

        public static async Task UploadBitmap(
            this CloudBlockBlob blob, Bitmap bitmap, ImageFormat format)
        {
            using (var stream = new MemoryStream())
            {
                switch (format)
                {
                    case ImageFormat.JPG:
                        bitmap.Save(stream, SDI.ImageFormat.Jpeg);
                        break;
                    case ImageFormat.GIF:
                        bitmap.Save(stream, SDI.ImageFormat.Gif);
                        break;
                    default:
                        bitmap.Save(stream, SDI.ImageFormat.Png);
                        break;
                }

                stream.Position = 0;

                await blob.UploadFromStreamAsync(stream);
            }
        }
    }
}
