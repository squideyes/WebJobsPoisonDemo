#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using System;
using System.ComponentModel.DataAnnotations;

namespace WebJobsPoisonDemo.CloudHelpers
{
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    public sealed class BlobNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var blobName = value as string;

            if (blobName == null)
                return true;

            return blobName.IsBlobName();
        }
    }
}
