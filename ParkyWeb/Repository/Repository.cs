using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _clientFactory;

        public Repository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<bool> CreateAsync(string url, T opjToCreate)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            if(opjToCreate != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(opjToCreate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }

            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<T> GetAsync(string url, int id)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, url+id);


            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else
            {
                return null;
            }
        }

        //public async Task<IEnumerable<T>> GetAllAsync(string url)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, url);


        //    var client = _clientFactory.CreateClient();
        //    HttpResponseMessage response = await client.SendAsync(request);

        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var jsonString = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }

            return null;
        }

        public async Task<bool> UpdateAsync(string url, T opjToUpdate)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);

            if (opjToUpdate != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(opjToUpdate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }

            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url+id);


            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
