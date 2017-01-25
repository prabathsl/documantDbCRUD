using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace DocuemntDb.Models
{
    public class DocuemntDbContext 
    {
        private const string _URI = "https://demoprabath.documents.azure.com:443/";
        private const string _PrimaryKey = "ZiMK9okPAaycGRY3wjiq3fcB5IVaq4oqDmQgwhtHBrCXLUPT2ECh6KQZVj9x6xEwDtNtzpbMd1kAh64PVopRYg==";
        private DocumentClient _Client;
        private string _dbName = "rjtDemo";
        private string _collectionName = "Student";

        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public DocuemntDbContext()
        {
            this._Client = new DocumentClient(new Uri(_URI), _PrimaryKey);
        }

        public async Task Init()
        {
            await CreateDatabaseIfNotExists(this._dbName);
            await CreateDocumentCollectionIfNotExists(this._dbName, this._collectionName);
        }

        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            try
            {
                await this._Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this._Client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }


        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await this._Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    await this._Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = 400 });

                }
                else
                {
                    throw;
                }
            }
        }


        internal async Task CreateDocumentIfNotExists(Student data)
        {
            try
            {
                await this._Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_dbName, _collectionName, data.id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this._Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName), data);
                }
                else
                {
                    throw;
                }
            }
        }


        internal List<Student> ExecuteFromLinq()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Student> lambdaQuery = this._Client.CreateDocumentQuery<Student>(
                    UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName), queryOptions)
                    .Where(f => f.Institute == "RJT");

            return lambdaQuery.ToList();
        }

        internal Student ExecuteFromSQL(string userId)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Student> queryInSql = this._Client.CreateDocumentQuery<Student>(
               UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName),
               "SELECT * FROM Student WHERE Student.Id = '"+ userId + "'",
               queryOptions);

            return queryInSql.FirstOrDefault();
        }

        internal async Task UpdateDocument(string userId,Student data)
        {
            try
            {
                await this._Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_dbName, _collectionName, userId), data);
            }
            catch (DocumentClientException de)
            {
                throw;
            }
        }

        internal async Task Deleteocument(string userId)
        {
            try
            {
                await this._Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_dbName, _collectionName, userId));
            }
            catch (DocumentClientException de)
            {
                throw;
            }
        }


        public System.Data.Entity.DbSet<DocuemntDb.Models.Student> Students { get; set; }
    }
}
