/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2014 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ADN.Toolkit.ReCap.DataContracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace Autodesk.ADN.Toolkit.ReCap
{
    /////////////////////////////////////////////////////////////////////////////////////
    // ADn ReCap Client
    //
    /////////////////////////////////////////////////////////////////////////////////////
    public class AdnReCapClient
    {
        private string _clientId;

        private RestClient _restClient;

        AdskRESTful _Client2;

        /////////////////////////////////////////////////////////////////////////////////
        // ReCap Client Constructor
        //
        /////////////////////////////////////////////////////////////////////////////////
        public AdnReCapClient(
           string serviceUrl,
           string clientID,
           string consumerKey,
           string consumerSecret,
           string accessToken,
           string accessTokenSecret)
        {
            _clientId = clientID;

            _restClient = new RestClient(serviceUrl);

            _restClient.Authenticator = 
                OAuth1Authenticator.ForProtectedResource(
                    consumerKey,
                    consumerSecret,
                    accessToken,
                    accessTokenSecret);


            _Client2 = new AdskRESTful (serviceUrl) ;
		    
            _Client2.addSubscriber (
                new AdskOauthPlugin (
                    new Dictionary<string, string> () {
				        { "oauth_consumer_key", consumerKey}, 
                        { "oauth_consumer_secret", consumerSecret },
				        { "oauth_token", accessToken }, 
                        { "oauth_token_secret", accessTokenSecret }
			        })) ;
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /service/date
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapServerTimeResponse> 
            GetServerTimeAsync()
        {
            var request = new RestRequest("service/date", Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapServerTimeResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /version
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapVersionResponse> 
            GetVersionAsync()
        {
            var request = new RestRequest("version", Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapVersionResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // POST /photoscene
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            CreatePhotosceneAsync(
                string sceneName,
                ReCapPhotosceneOptionsBuilder options)
        {
            var request = new RestRequest("photoscene", Method.POST);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("scenename", sceneName);
            request.AddParameter("json", 1);

            if (options != null)
            {
                var opts = options.ToPhotosceneOptions();

                foreach (var entry in opts)
                    request.AddParameter(entry.Key, entry.Value);
            }

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            GetPhotoscenePropertiesAsync(
                string photosceneId)
        {
            var request = new RestRequest(
                string.Format("photoscene/{0}/properties", photosceneId),
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            var response = await _restClient.ExecuteAsync
                 <ReCapPhotoscenePropsResponse>(
                    request);

            return response.ToPhotosceneResponse();
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /photoscene/{photosceneid}/progress 
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            GetPhotosceneProgressAsync(
                string photosceneId)
        {
            var request = new RestRequest(
                string.Format("photoscene/{0}/progress", photosceneId),
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /photoscene/{photosceneid}/processingtime
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            GetPhotosceneProcessingTimeAsync(
                string photosceneId)
        {
            var request = new RestRequest(
                string.Format("photoscene/{0}/processingtime", photosceneId),
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneListResponse>
            GetPhotosceneListAsync()
        {
            var request = new RestRequest(
                "photoscene/properties",
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneListResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneListResponse> 
            GetPhotosceneListByUserIdAsync(
                string userId)
        {
            var request = new RestRequest(    
                "photoscene/properties",
                Method.GET);
 
            request.AddParameter("clientID", _clientId);
            request.AddParameter("attributeName", "userID");
            request.AddParameter("attributeValue", HttpUtility.UrlDecode(userId));
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneListResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // POST /file
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapFileResponse> 
            UploadFilesAsync(
                string photosceneId,
                string[] files)
        {
            var request = new RestRequest("file", Method.POST);

            request.Timeout = 1 * 60 * 60 * 1000;

            request.AddParameter("clientID", _clientId);
            request.AddParameter("photosceneid", photosceneId);
            request.AddParameter("type", "image");
            request.AddParameter("json", 1);

            int idx = 0;

            foreach (var filename in files)
            {
                string key = string.Format("file[{0}]", idx++);
                request.AddFile(key, filename);
            }

            return await _restClient.ExecuteAsync
                <ReCapFileResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /file/{fileid}/get 
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapFileResponse> 
            GetFileLinkAsync(
                string fileId)
        {
            var request = new RestRequest(
                string.Format("file/{0}/get", fileId),
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("type", "image");
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapFileResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // POST /photoscene/{photosceneid} 
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            ProcessPhotosceneAsync(
                string photosceneId,
                bool forceReprocess = true)
        {
            var request = new RestRequest(
                string.Format("photoscene/{0}", photosceneId),
                Method.POST);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("forceReprocess", (forceReprocess ? "1" : "0"));
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET /photoscene/{photosceneid} 
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            GetPhotosceneLinkAsync(
                string photosceneId,
                MeshFormatEnum meshFormat)
        {
            var request = new RestRequest(
                string.Format("photoscene/{0}", photosceneId),
                Method.GET);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("format", meshFormat.ToReCapString());
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        //
        /////////////////////////////////////////////////////////////////////////////////
        //public async Task<ReCapPhotoSceneDeleteResponse> 
        //    DeletePhotosceneAsync(
        //        string photosceneId)
        //{
        //    var request = new RestRequest(
        //        string.Format("photoscene/{0}", photosceneId), 
        //        Method.DELETE);

        //    request.AddParameter("clientID", _clientId);
        //    request.AddParameter("json", 1);

        //    return await _restClient.ExecuteAsync
        //        <ReCapPhotoSceneDeleteResponse>(
        //            request);
        //}

        public async Task<ReCapPhotosceneDeleteResponse>
            DeletePhotosceneAsync(
                string photosceneId)
        {
            ReCapPhotosceneDeleteResponse reCapResponse =
                new ReCapPhotosceneDeleteResponse();

            try
            {
                _Client2.AlwaysMultipart = true;
                _Client2.AlwaysSignParameters = true;

                _Client2.addParameter("clientID", _clientId);
                _Client2.addParameter("json", "1");

                HttpWebRequest req = _Client2.delete(
                    string.Format("photoscene/{0}", photosceneId), null);

                HttpWebResponse response = await _Client2.sendAsync(req);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    reCapResponse.Error = new ReCapError(
                        response.StatusCode);

                    return reCapResponse;
                }

                using (System.IO.StreamReader reader =
                    new System.IO.StreamReader(
                        response.GetResponseStream()))
                {
                    string text = reader.ReadToEnd();

                    List<ErrorEventArgs> jsonErrors =
                        new List<ErrorEventArgs>();

                    reCapResponse =
                       JsonConvert.DeserializeObject
                       <ReCapPhotosceneDeleteResponse>(
                           text,
                            new JsonSerializerSettings
                            {
                                Error = (
                                    object sender,
                                    ErrorEventArgs args) =>
                                {
                                    args.ErrorContext.Handled = true;

                                    jsonErrors.Add(args);
                                }
                            });

                    if (jsonErrors.Count != 0)
                    {
                        reCapResponse.Error =
                            new ReCapError(jsonErrors);
                    }

                    return reCapResponse;
                }
            }
            catch (Exception ex)
            {
                reCapResponse.Error =
                    new ReCapError(ex);

                return reCapResponse;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        // POST /notification/template 
        //
        /////////////////////////////////////////////////////////////////////////////////
        public async Task<ReCapPhotosceneResponse> 
            SendNotificationAsync(
                string emailText,
                bool error = false)
        {
            var request = new RestRequest("notification/template", Method.POST);

            request.AddParameter("clientID", _clientId);
            request.AddParameter("emailTxt", emailText);
            request.AddParameter("emailType", (error ? "ERROR" : "DONE"));
            request.AddParameter("json", 1);

            return await _restClient.ExecuteAsync
                <ReCapPhotosceneResponse>(
                    request);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////
    // Custom Extensions for RestClient
    //
    /////////////////////////////////////////////////////////////////////////////////
    public static class RestClientExtensions
    {
        public static async Task<IRestResponse> ExecuteAsync(
            this RestClient client,
            RestRequest request)
        {
            return await Task<IRestResponse>.Factory.StartNew(() =>
            {
                return client.Execute(request);
            });
        }

        public static async Task<T> ExecuteAsync<T>(
            this RestClient client,
            RestRequest request) where T : new()
        {
            return await Task<T>.Factory.StartNew(() =>
            {
                try
                {
                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        dynamic reCapError = new T();

                        reCapError.Error = new ReCapError(
                            response.StatusCode);

                        return reCapError;
                    }

                    List<ErrorEventArgs> jsonErrors = 
                        new List<ErrorEventArgs>();

                    T reCapResponse =
                       JsonConvert.DeserializeObject<T>(
                           response.Content,
                            new JsonSerializerSettings
                            {
                                Error = (
                                    object sender, 
                                    ErrorEventArgs args) =>
                                {
                                    args.ErrorContext.Handled = true;

                                    jsonErrors.Add(args);
                                }
                            });

                    if (jsonErrors.Count != 0)
                    {
                        dynamic reCapResponseWithErrors =
                            (reCapResponse != null ? reCapResponse : new T());

                        reCapResponseWithErrors.Error =
                            new ReCapError(jsonErrors);

                        return reCapResponseWithErrors;
                    }

                    return reCapResponse;
                }
                catch (Exception ex)
                {
                    dynamic reCapError = new T();

                    reCapError.Error = 
                        new ReCapError(ex);

                    return reCapError;
                }
            });
        }
    }
}
