#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using WebJobsPoisonDemo.Demo.Shared;
using System.Drawing;
using System.IO;
using SDI = System.Drawing.Imaging;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public static class BitmapExtender
    {
        public static byte[] ToArray(
            this Image image, ImageFormat kind = ImageFormat.PNG)
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                switch (kind)
                {
                    case ImageFormat.GIF:
                        image.Save(stream, SDI.ImageFormat.Gif);
                        break;
                    case ImageFormat.JPG:
                        image.Save(stream, SDI.ImageFormat.Jpeg);
                        break;
                    default:
                        image.Save(stream, SDI.ImageFormat.Png);
                        break;
                }

                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
