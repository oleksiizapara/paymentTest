using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Payment
{
    class Program
    {

        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        private string EndpointUrl = "https://cosmospaymenttest.documents.azure.com:443/";

        /// The primary key for the Azure DocumentDB account.
        private string PrimaryKey = "c1xKfVitOV8IDJB7hyIutRLs80guAHKLNMi8qATPgiuzZbBFiJh1eK6qgzvcLY5nBDDgzr14AqfxGIpmrQW3Rw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;


        // The name of the database and container we will create
        private string databaseId = "Demo";
        private string containerId = "payment";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }

        }

        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey, new CosmosClientOptions {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    IgnoreNullValues = true,
                },
                ConnectionMode = ConnectionMode.Direct,
            });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
            await this.QueryItemsAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Uetr");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync()
        {
            var uetr = Guid.NewGuid();

            var payment = new Payment
            {
                Id = uetr.ToString(),
                Uetr = uetr.ToString(),
                CancellationRequest = new CancellationRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = RequestStatus.Approved,
                    cancellationApprovals = new List<PaymentApproval>
                    {
                        new PaymentApproval
                        {
                            Id =  Guid.NewGuid().ToString(),
                            Status = ApprovalStatus.Approved,
                            ApprovedAt = DateTimeOffset.UtcNow,
                            Reason = "bla bla",
                        }
                    }
                }
            };

            try
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen".
                ItemResponse<Payment> andersenFamilyResponse = await this.container.CreateItemAsync<Payment>(payment, new PartitionKey(payment.Uetr));
                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", payment.Id);
            }
        }

        private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            var sd = this.container.GetItemLinqQueryable<Payment>(true).Where(x => x.CancellationRequest != null && x.CancellationRequest.Status == RequestStatus.Approved);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Payment> queryResultSetIterator = this.container.GetItemQueryIterator<Payment>(queryDefinition);

            List<Payment> families = new List<Payment>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Payment> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Payment family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }

    }
}
