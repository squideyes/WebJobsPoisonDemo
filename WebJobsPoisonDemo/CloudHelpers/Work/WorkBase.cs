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
    public abstract class WorkBase
    {
        public void Validate() =>
            Validator.ValidateObject(this, new ValidationContext(this));
    }
}
