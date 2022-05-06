﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Peernet.SDK.Client.Http;
using Peernet.SDK.Models.Domain.Common;
using Peernet.SDK.Models.Domain.File;

namespace Peernet.SDK.Client.Clients
{
    internal class FileClient : ClientBase, IFileClient
    {
        private readonly IHttpExecutor httpExecutor;

        public FileClient(IHttpExecutor httpExecutor)
        {
            this.httpExecutor = httpExecutor;
        }

        public override string CoreSegment => "file";

        public async Task<ApiResponseFileFormat> GetFormat(string path)
        {
            var parameters = new Dictionary<string, string>
            {
                [nameof(path)] = path
            };

            return await httpExecutor.GetResultAsync<ApiResponseFileFormat>(HttpMethod.Get, GetRelativeRequestPath("format"),
                parameters);
        }

        public async Task<Stream> Read(ApiFile file)
        {
            var parameters = new Dictionary<string, string>
            {
                ["hash"] = Convert.ToHexString(file.Hash),
                ["node"] = Convert.ToHexString(file.NodeId),
            };

            return await httpExecutor.GetAsync(HttpMethod.Get, GetRelativeRequestPath("read"), parameters);
        }
    }
}