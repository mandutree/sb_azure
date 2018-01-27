using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using ContactsList.API.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ContactsList.API.Storage
{
    public class DocumentDb
    {
        private DocumentClient client;

        private DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    var url = ConfigurationManager.AppSettings["cosmosDbUrl"];
                    var primaryKey = ConfigurationManager.AppSettings["cosmosDbPk"];
                    client = new DocumentClient(new Uri(url), primaryKey);
                }

                return client;

            }
        }

        public async Task<IEnumerable<Contact>> Get()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = 10 };
            var query = Client.CreateDocumentQuery<Contact>(UriFactory.CreateDocumentCollectionUri("user_profile", "contacts"), queryOptions);

            var contacts = new List<Contact>();

            try
            {
                foreach (Contact contact in query)
                {
                    contacts.Add(contact);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return contacts;
        }

        public async Task Save(IEnumerable<Contact> contacts)
        {
            foreach(var contact in contacts)
            {
                try
                {
                    Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("user_profile", "contacts"), contact).Wait();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }
    }
}