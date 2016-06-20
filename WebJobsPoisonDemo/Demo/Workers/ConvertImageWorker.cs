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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SDI = System.Drawing.Imaging;

namespace WebJobsPoisonDemo.Demo.Workers
{
    public class ConvertImageWorker : WorkerBase<ConvertImageWork>
    {
        public ConvertImageWorker() 
            : base(3)
        {
        }

        protected override async Task HandleWork(CloudStorageAccount account,
            CancellationToken token, ConvertImageWork work)
        {
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(
                WellKnown.ContainerNames.Observatories);

            var source = container.GetBlockBlobReference(work.Source);

            var image = Image.FromStream(await source.GetStreamAsync());

            using (var stream = new MemoryStream())
            {
                switch (work.Format)
                {
                    case ImageFormat.JPG:
                        var encoder = GetEncoder(SDI.ImageFormat.Jpeg);
                        var encodeParams = new SDI.EncoderParameters(1);
                        encodeParams.Param[0] = new SDI.EncoderParameter(
                            SDI.Encoder.Quality, work.JpgQuality);
                        image.Save(stream, encoder, encodeParams);
                        break;
                    case ImageFormat.PNG:
                        image.Save(stream, SDI.ImageFormat.Png);
                        break;
                    case ImageFormat.GIF:
                        image.Save(stream, SDI.ImageFormat.Gif);
                        break;
                }

                stream.Position = 0;

                var target = container.GetBlockBlobReference(work.Target);

                await target.UploadFromStreamAsync(stream, token);
            }
        }

        protected override string GetSuccessLogMessage(ConvertImageWork work)
        {
            var sb = new StringBuilder();

            sb.Append("Converted \"");
            sb.Append(work.Source);
            sb.Append("\" to \"");
            sb.Append(work.Target);
            sb.Append("\" (Format: ");
            sb.Append(work.Format.ToString().ToUpper());

            if (work.Format == ImageFormat.JPG)
                sb.Append($", Quality: {work.JpgQuality}");

            sb.Append(")");

            return sb.ToString();
        }

        private SDI.ImageCodecInfo GetEncoder(SDI.ImageFormat format)
        {
            foreach (var codec in SDI.ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }

            return null;
        }
    }
}
