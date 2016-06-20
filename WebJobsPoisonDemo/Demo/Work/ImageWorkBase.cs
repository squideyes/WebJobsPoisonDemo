#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using WebJobsPoisonDemo.CloudHelpers;
using WebJobsPoisonDemo.Demo.Shared;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebJobsPoisonDemo.Demo.Work
{
    public abstract class ImageWorkBase : WorkBase
    {
        [Required]
        [BlobName]
        public string Source { get; set; }

        [Required]
        [BlobName]
        public string Target { get; set; }

        [Required]
        [DefaultValue(ImageFormat.PNG)]
        public ImageFormat Format { get; set; }
    }
}
