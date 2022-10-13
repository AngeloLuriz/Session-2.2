using System;
using System.Net;
using RestSharp;

namespace Session_2._2
{
    [TestClass]
    public class UnitTest1
    {

        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetClass> cleanUpList = new List<PetClass>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {

            List<Tag> tags = new List<Tag>();
            tags.Add(new Tag()
            {
                Id = 1001,
                Name = "SampleTag"
            });

            PetClass petData = new PetClass()
            {
                Id = 2,
                Name = "Updated Name",
                PhotoUrls = new string[1] { "Updated PhotoURL" },
                Category = new Category { Id = 1, Name = "Updated Category" },
                Tags = new Category[1] { new Category { Id = 1, Name = "Updated Tags" } },
                Status = "available"
            };
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(petData);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{petData.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetClass>(restRequest);

           //Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(petData.Name, restResponse.Data.Name, "Name did not match.");
            Assert.AreEqual(petData.Category.Id, restResponse.Data.Category.Id, "Category ID did not match.");
            Assert.AreEqual(petData.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoUrls did not match.");
            Assert.AreEqual(petData.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags did not match.");
            Assert.AreEqual(petData.Status, restResponse.Data.Status, "Status did not match.");

            cleanUpList.Add(petData);
        }
    }
}