/**
 * Copyright 2019 The Jambox Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Jambox.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Jambox.Common.TinyJson;
    using Jambox.Tourney.Server;
    using UnityEngine;

    /// <summary>
    /// HTTP Request adapter which uses the .NET HttpClient to send requests.
    /// </summary>
    /// <remarks>
    /// Accept header is always set as 'application/json'.
    /// </remarks>
    public class HttpRequestAdapter : IHttpAdapter
    {
        /// <inheritdoc cref="IHttpAdapter.Logger"/>
        public ILogger Logger { get; set; }

        private readonly HttpClient _httpClient;

        public HttpRequestAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // remove cap of max timeout on HttpClient from 100 seconds.
            _httpClient.Timeout = TimeSpan.FromSeconds(20d);
        }

        /// <inheritdoc cref="IHttpAdapter"/>
        public async Task<String> SendAsync(string method, Uri uri, IDictionary<string, string> headers, byte[] body,
            int timeout, bool continueOnError = false)
        {
            //try
            //{
                UnityDebug.Debug.Log("SendAsync Of Httprequest Hit >>> Uri : " + uri.ToString());
                var request = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Method = new HttpMethod(method),
                    Headers =
                    {
                        Accept = {new MediaTypeWithQualityHeaderValue("application/json")}
                    }
                };

                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }

                if (body != null)
                {
                    request.Content = new ByteArrayContent(body);
                }

                var timeoutToken = new CancellationTokenSource();
                timeoutToken.CancelAfter(TimeSpan.FromSeconds(timeout));

                UnityDebug.Debug.Log("Send: method : " + method + " uri : " + uri.ToString() + "  body :   " + body  + "   timeout : " + timeout);
            
                var response = await _httpClient.SendAsync(request, timeoutToken.Token);
                var contents = await response.Content.ReadAsStringAsync();
                response.Content?.Dispose();


                UnityDebug.Debug.Log("Received: status : " + response.StatusCode + " from : " + uri.ToString() + "\nContents : " + contents);


            if (response.IsSuccessStatusCode)
            {
                var decodedNew = contents.FromJson<Dictionary<string, object>>();
                if (decodedNew.ContainsKey("error"))
                {
                    string errorMsg = decodedNew["error"].ToString();
                    throw new Exception(errorMsg);
                }
                UnityDebug.Debug.Log("SendAsync Of Httprequest Hit >>> 222 Success hit : ");
                return contents;
            }
            else
            {
                UnityDebug.Debug.Log("Else case of packet Parsing Hit >>>>>");
                string message = "";
                if (contents != null)
                {
                    //UnityDebug.Debug.Log("Else case of packet Parsing Hit 00000>>>>>");
                    var decoded = contents.FromJson<Dictionary<string, object>>();
                    //UnityDebug.Debug.Log("Else case of packet Parsing Hit 11111>>>>>");
                    if (decoded != null && decoded.Keys.Count > 0)
                    {
                        message = decoded.ContainsKey("message") ? decoded["message"].ToString() : string.Empty;
                        //UnityDebug.Debug.Log("Else case of packet Parsing Hit 1111-2222>>>>>");
                        int grpcCode = decoded.ContainsKey("code") ? (int)decoded["code"] : -1;
                        //UnityDebug.Debug.Log("Else case of packet Parsing Hit 222222>>>>>");
                        //var exception = new ApiResponseException((int) response.StatusCode, message, grpcCode);

                        if (decoded.ContainsKey("error"))
                        {
                            object data;
                            decoded.TryGetValue("error", out data);
                            //UnityDebug.Debug.LogError("SendAsync Of Httprequest Hit >>> Error : " + data.ToString());
                            //IHttpAdapterUtil.CopyResponseError(this, decoded["error"], exception);
                            //IHttpAdapterUtil.SendError(data.ToString(), continueOnError);
                            message = data.ToString();
                        }
                    }
                }
                //UnityDebug.Debug.Log("Else case of packet Parsing Hit >>>>> 33333");
                if (String.IsNullOrEmpty(message))
                    message = "Our servers are not responding. Please try after sometime.";
                UnityDebug.Debug.LogError("SendAsync Of Httprequest Hit >>> exception : " + message);
                throw new Exception(message);
            }

            //throw exception;
            //}
            //catch (HttpRequestException e)
            //{
            //    Debug.LogError("Exception Caught! >>>>>>");
            //    Debug.LogError("Message : " + e.Message);
            //    var exception = new ApiResponseException(104, "unknown", 311);
            //    IHttpAdapterUtil.SendError("We are unable to connect to our server. Please try after some time.", continueOnError);
            //    Debug.LogError("SendAsync Of Httprequest Hit >>> exception : ");
            //    throw exception;
            //}
            //catch (WebException e)
            //{
            //    Debug.LogError("WebException Exception Caught! >>>>>>");
            //    Debug.LogError("Message : " + e.Message);
            //    var exception = new ApiResponseException(104, "unknown", 311);
            //    IHttpAdapterUtil.SendError("", continueOnError);
            //    Debug.LogError("SendAsync Of Httprequest Hit >>> exception : ");
            //    throw exception;
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("Exception Caught! >>>>>> " + e.Message);
            //    Debug.LogError("Exception Caught! >>>>>> " + e.StackTrace);
            //    var exception = new ApiResponseException(104, "unknown", 311);
            //    IHttpAdapterUtil.SendError("We are unable to connect to our server. Please try after some time", continueOnError);
            //    throw exception;
            //}
        }
        /// <summary>
        /// A new HTTP adapter with configuration for gzip support in the underlying HTTP client.
        /// </summary>
        /// <remarks>
        /// NOTE Decompression does not work with Mono AOT on Android.
        /// </remarks>
        /// <param name="decompression">If automatic decompression should be enabled with the HTTP adapter.</param>
        /// <param name="compression">If automatic compression should be enabled with the HTTP adapter.</param>
        /// <returns>A new HTTP adapter.</returns>
        public static IHttpAdapter WithGzip(bool decompression = false, bool compression = false)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression && decompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var client =
                new HttpClient(compression ? (HttpMessageHandler) new GZipHttpClientHandler(handler) : handler);
            return new HttpRequestAdapter(client);
        }
    }
}
