#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using System.ComponentModel.DataAnnotations;

namespace WebJobsPoisonDemo.Demo.Work
{
    public class ResizeImageWork : ImageWorkBase
    {
        public const string QueueName = "resizeimages";

        [Required]
        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Height { get; set; }
    }
}
