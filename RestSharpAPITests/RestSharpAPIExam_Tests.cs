using RestSharp;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://contactbook.kameliya-m.repl.co/api";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }
        [Test]
        public void Test_GetAllContacts()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(contacts[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Jobs"));

        }
        [Test]
        public void Test_GetContactByKeyWord_ValidResult()
        {
            // Arrange
            var request = new RestRequest("contacts/search/albert", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(contacts[0].firstName, Is.EqualTo("Albert"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Einstein"));

        }
        [Test]
        public void Test_GetContactByKeyWord_InvalidResult()
        {
            // Arrange
            var request = new RestRequest("contacts/search/missing" + DateTime.Now.Ticks, Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("[]"));


         }
        [Test]
        public void Test_CreateContact_InvalidResult()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                lastName =  "Curie",
                email =  "marie67@gmail.com",
               phone = "+1 800 200 300",
               dateCreated = "2023-02-26T08:48:54.309Z",
               comments  = "Old friend"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);

            // Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));


        }
        [Test]
        public void Test_CreateContact_ValidBody()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                firstName = "Marie",
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                dateCreated = "2023-02-26T08:48:54.309Z",
                comments = "Old friend"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);
            var data = JsonSerializer.Deserialize<contactObject>(response.Content);
            // Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            Assert.That(data.contact.firstName, Is.EqualTo(reqBody.firstName));
            Assert.That(data.contact.lastName, Is.EqualTo(reqBody.lastName));
            Assert.That(data.contact.email, Is.EqualTo(reqBody.email));
            Assert.That(data.contact.phone, Is.EqualTo(reqBody.phone));
            Assert.That(data.contact.dateCreated, Is.Not.Empty);
            Assert.That(data.contact.comments, Is.EqualTo(reqBody.comments));

        }
    }
}