using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luma.Entities.Services
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://luma.lacuna.cc/";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public void SetAccessToken(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<StartApiResponse> StartApi(string username, string email)
        {
            var url = $"{BaseUrl}api/start";
            var objeto = new 
            {
                username = username,
                email = email 
            };
            var json = JsonConvert.SerializeObject(objeto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(url, content);
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            StartApiResponse startApiResponse = JsonConvert.DeserializeObject<StartApiResponse>(responseString);
            if(startApiResponse.Code == "Success")
            {
                SetAccessToken(startApiResponse.AccessToken);
                return startApiResponse;
            }
            else
            {
                throw new Exception(startApiResponse.Message);
            }
        }
        public async Task<ProbesClass> ListProbes()
        {
            var url = $"{BaseUrl}api/probe";
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(url);
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            ProbesClass probesClass = JsonConvert.DeserializeObject<ProbesClass>(responseString);
            if(probesClass.Code == "Success")
            {
                return probesClass;
            }
            else
            {
                throw new Exception(probesClass.Message);
            }
        }
        public async Task<ProbeSyncResponse> SyncProbe(string id)
        {
            var url = $"{BaseUrl}api/probe/{id}/sync";
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(url, null);
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            ProbeSyncResponse probeSyncResponse = JsonConvert.DeserializeObject<ProbeSyncResponse>(responseString);
            if(probeSyncResponse.Code == "Success")
            {
                return probeSyncResponse;
            }
            else
            {
                throw new Exception(probeSyncResponse.Message);
            }
        }
        public async Task<JobResponse> takeJob()
        {
            var url = $"{BaseUrl}api/job/take";
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(url, null);
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            JobResponse jobResponse = JsonConvert.DeserializeObject<JobResponse>(responseString);
            if(jobResponse.Code == "Success")
            {
                return jobResponse;
            }
            else
            {
                throw new Exception(jobResponse.Message);
            }
        }
        public async Task<GenericResponse> checkJob(string id, string probeNow, long roundTrip)
        {
            var url = $"{BaseUrl}api/job/{id}/check";
            var objeto = new
            {
                probeNow = probeNow,
                roundTrip = roundTrip
            };
            var json = JsonConvert.SerializeObject(objeto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(url, content);
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            GenericResponse genericResponse = JsonConvert.DeserializeObject<GenericResponse>(responseString);
            return genericResponse;

        }
    }
}
