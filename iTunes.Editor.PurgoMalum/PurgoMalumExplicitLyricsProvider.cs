﻿// -----------------------------------------------------------------------
// <copyright file="PurgoMalumExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PurgoMalum
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using RestSharp;

    /// <summary>
    /// The <see cref="IExplicitLyricsProvider"/> using Purgo Malum.
    /// </summary>
    public class PurgoMalumExplicitLyricsProvider : IExplicitLyricsProvider
    {
        private readonly IRestClient client = new RestClient("https://www.purgomalum.com/service/");

        /// <inheritdoc/>
        public bool? IsExplicit(string lyrics) => ParseResponse(this.client.Execute(GetRequest(lyrics)));

        /// <inheritdoc/>
        public async Task<bool?> IsExplicitAsync(string lyrics) => ParseResponse(await this.client.ExecuteTaskAsync(GetRequest(lyrics)).ConfigureAwait(false));

        private static IRestRequest GetRequest(string lyrics)
        {
            var request = new RestRequest("containsprofanity", Method.GET);
            request.AddParameter("text", lyrics);
            request.AddHeader("Accept", "text/html, application/xhtml+xml, application/xml, text/plain");
            return request;
        }

        private static bool? ParseResponse(IRestResponse response)
        {
            if (bool.TryParse(response.Content, out var boolValue))
            {
                return boolValue;
            }

            return null;
        }
    }
}