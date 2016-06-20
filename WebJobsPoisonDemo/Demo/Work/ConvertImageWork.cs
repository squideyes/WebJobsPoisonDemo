#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebJobsPoisonDemo.Demo.Work
{
    public class ConvertImageWork : ImageWorkBase
    {
        public const string QueueName = "convertimages";

        [Range(0, 100)]
        [DefaultValue(95)]
        public int JpgQuality { get; set; }
    }
}
