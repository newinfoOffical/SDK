﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Peernet.SDK.Client.Http;
using Peernet.SDK.Models.Domain.Warehouse;

namespace Peernet.SDK.Client.Clients
{
    internal class WarehouseClient : ClientBase, IWarehouseClient
    {
        private readonly IHttpExecutor httpExecutor;

        public WarehouseClient(IHttpExecutor httpExecutor)
        {
            this.httpExecutor = httpExecutor;
        }

        public override string CoreSegment => "warehouse";

        public async Task<WarehouseResult> Create(Stream stream, IProgress<UploadProgress> progress, CancellationToken cancellationToken = default)
        {
            using var content = new ProgressableStreamContent(stream, progress);

            return await httpExecutor.GetResultAsync<WarehouseResult>(HttpMethod.Post, GetRelativeRequestPath("create"),
                content: content, cancellationToken: cancellationToken);
        }

        public async Task<WarehouseResult> ReadPath(byte[] hash, string path)
        {
            var parameters = new Dictionary<string, string>
            {
                [nameof(hash)] = Convert.ToHexString(hash),
                [nameof(path)] = path
            };

            return await httpExecutor.GetResultAsync<WarehouseResult>(HttpMethod.Get, GetRelativeRequestPath("read/path"),
                parameters);
        }
    }
}