﻿using Peernet.SDK.Client.Clients;
using Peernet.SDK.Models.Domain.Common;
using Peernet.SDK.Models.Domain.Download;
using System;
using System.Threading.Tasks;

namespace Peernet.SDK.Models.Presentation
{
    public class Download : DataTransfer
    {
        private readonly IDownloadClient downloadClient;

        public Download(IDownloadClient downloadClient, ApiFile file, string path)
            : base()
        {
            this.downloadClient = downloadClient; 
            File = file;
            DestinationPath = path;
        }

        public ApiFile File { get; set; }

        public string DestinationPath { get; private set; }

        public DownloadStatus Status { get; set; }

        public override string Name => File.Name.Length > 26 ? $"{File.Name.Substring(0, 26)}..." : File.Name;

        public override event EventHandler Completed;
        public override event EventHandler StatusChanged;
        public override event EventHandler ProgressChanged;

        public override async Task<ApiResponseDownloadStatus> Cancel()
        {
            var status = await Execute(DownloadAction.Cancel);

            return status;
        }

        public override async Task<ApiResponseDownloadStatus> Pause()
        {
            return await Execute(DownloadAction.Pause);
        }

        public override async Task<ApiResponseDownloadStatus> Resume()
        {
            return await Execute(DownloadAction.Resume);
        }

        public override async Task<ApiResponseDownloadStatus> Start()
        {
            var responseStatus = await downloadClient.Start(DestinationPath, File.Hash, File.NodeId);
            Id = responseStatus.Id;

            return responseStatus;
        }

        public override async Task UpdateStatus()
        {
            var status = await downloadClient.GetStatus(Id);
            if (status != null)
            {
                if (status.DownloadStatus == DownloadStatus.DownloadFinished)
                {
                    IsCompleted = true;
                    Progress = 100;
                    Completed?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Progress = status.Progress.Percentage;
                }
                
                ProgressChanged?.Invoke(this, EventArgs.Empty);
                Status = status.DownloadStatus;
            }
        }

        private async Task<ApiResponseDownloadStatus> Execute(DownloadAction action)
        {
            return await downloadClient.GetAction(Id, action);
        }
    }
}
