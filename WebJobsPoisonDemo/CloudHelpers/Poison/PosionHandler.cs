#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public static class PoisonHandler
    {
        public static async Task SaveAsBlobThenEmailAlerts(string from, List<string> tos,
            TextWriter log, SendGridMessage message, PoisonInfo info, CloudBlockBlob blob)
        {
            var infoAsJson = JsonConvert.SerializeObject(info);

            await blob.UploadTextAsync(infoAsJson);

            await log.WriteLineAsync($"Saved poison-info to {info.BlobName}");

            message.From = new MailAddress(from);

            foreach (var email in tos)
                message.AddTo(email);

            message.Subject = $"POISON MESSAGE DETECTED on the \"{info.QueueName}\" queue";

            var sb = new StringBuilder();

            AddTokenValue(sb, "ID:", info.PoisonInfoId.ToString());
            AddTokenValue(sb, "QUEUE NAME:", info.QueueName);
            AddTokenValue(sb, "WORK TYPE:", info.WorkType);
            AddTokenValue(sb, "DETECTED ON:", info.DetectedOn.ToString("MM/dd/yyyy HH:mm:ss.fff"));
            sb.AppendLine();
            sb.AppendLine("WORK:");
            sb.AppendLine(JObject.Parse(info.WorkAsJson).ToString(Formatting.Indented));
            sb.AppendLine();
            sb.AppendLine("ERROR:");
            sb.AppendLine(JsonConvert.SerializeObject(info.ErrorInfo, Formatting.Indented));

            message.Text = sb.ToString();

            AddAttachment(message, info.WorkAsJson, "Work.json");

            AddAttachment(message, infoAsJson, info.BlobName);
        }

        private static void AddAttachment(SendGridMessage message, string json, string fileName)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                writer.Write(json);

                writer.Flush();

                stream.Position = 0;

                message.AddAttachment(stream, fileName);
            }
        }

        private static void AddTokenValue(StringBuilder sb, string token, string value) =>
            sb.AppendLine($"{token}  {value}");
    }
}
