﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XForms.Constants;
using XForms.Enum;

namespace XForms.HttpREST
{
    public static class RESTHelper
    {
        public static async Task<RESTServiceResponse<T>> GetRequest<T>(string url, HttpVerbs method = HttpVerbs.GET, System.Collections.Specialized.NameValueCollection getParams = null, object postObject = null, bool isNeedAcces = true, string contentType = "application/json")
        {
            try
            {
                 //Skip Certificate Validation
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                using (var client = new HttpClient(clientHandler))
                {
                    Uri uri = new Uri(url);
                    client.BaseAddress = uri;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                    if (isNeedAcces)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AppPreferences.Token);
                    }

                    HttpResponseMessage response = new HttpResponseMessage();
                    switch (method)
                    {
                        case HttpVerbs.GET:
                            response = await client.GetAsync(uri);
                            break;
                        case HttpVerbs.POST:
                            var content = new StringContent(JsonConvert.SerializeObject(postObject), Encoding.UTF8, contentType);
                            response = await client.PostAsync(uri, content);
                            break;
                        case HttpVerbs.DELETE:
                            response = await client.DeleteAsync(uri);
                            break;
                        default:
                            break;
                    }

                    var stringResponseJson = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<RESTServiceResponse<T>>(stringResponseJson);

                    return result;
                }
            }
            catch(Exception Ex)
            {
                return new RESTServiceResponse<T>(false, Ex.Message);
            }
        }
        public static async Task<RESTServiceResponse<T>> UploadFileAsync<T>(string url, Models.File fileData, Dictionary<string, string> stringContent = null)
        {
            try
            {
                //#region IsConnected
                //if (!AppHelpers.IsConnected())
                //{
                //    return new RESTServiceResponse<T>(false, "Vous n'êtes pas connéctés !");
                //}
                //#endregion

                //if (!IsPJFileAuthorized(fileData.FileExtension))
                //{
                //    AppsHelper.Snack("Le format du fichier non supporté");
                //    return new RESTServiceResponse<DtoAttacherPJResponseApi>(false, "Le format du fichier non supporté");
                //    //return true;
                //}

                //var acceptedSize = (long)Constant.MAX_FILE_UPLOAD_WS * 1024 * 1024 * 1024;//convert to Mo

                //if (stream.Length > acceptedSize)
                //{
                //    Debug.WriteLine("la taille de fichier doit pas dépasser 10 mo");
                //    //return;
                //}

                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

                //var handler = new HttpClientHandler() { CookieContainer = App.MRHSessionCookies };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AppPreferences.Token);
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("APIKEY", AppUrls.APIKEY);

                    MultipartFormDataContent formData = new MultipartFormDataContent();
                    HttpContent fileStreamContent = new StreamContent(fileData.Stream);

                    fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "FileByte",
                        //FileName = fileData.Name
                    };
                    fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    formData.Add(fileStreamContent);

                    if (stringContent != null)
                    {
                        foreach (var item in stringContent)
                        {
                            formData.Add(new StringContent(item.Value), item.Key);
                        }
                    }

                    var response = await client.PostAsync(url, formData);

                    var responseMessage = await response.Content.ReadAsStringAsync();

                    RESTServiceResponse<T> result = JsonConvert.DeserializeObject<RESTServiceResponse<T>>(responseMessage);

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new RESTServiceResponse<T>(false, ex.Message);
            }
        }

    }
}
