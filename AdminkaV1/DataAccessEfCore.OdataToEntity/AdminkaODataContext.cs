//using Microsoft.OData.Edm;
//using OdataToEntity;
//using OdataToEntity.Db;
//using System;
//using System.IO;
//using System.Threading;

//namespace DashboardCode.AdminkaV1.DataAccessEfCore.OdataToEntity
//{
//    /// <summary>
//    /// OdataToEntity can't be referenced in Standard lib (only in .NET Framework)
//    /// More information: https://github.com/voronov-maxim/OdataToEntity
//    /// </summary>

//    public class AdminkaODataContext
//    {
//        readonly OeParser parser;
//        public AdminkaODataContext(string baseUri, IAdminkaOptionsFactory optionsFactory)
//        {
//            var dataAdapter = new AdminkaOeEfCoreDataAdapter(optionsFactory);
//            //Build OData Edm Model
//            EdmModel edmModel = dataAdapter.BuildEdmModel();
//            //Create query parser
//            this.parser = new OeParser(new Uri(baseUri /*"http://dummy"*/), dataAdapter, dataAdapter.BuildEdmModel());
//        }

//        public async void AppendStream(Stream responceStream, Uri query)
//        {
//            //Query
//            var uri = new Uri("http://dummy/Users?$select=Name");
//            //The result of the query
//            //Execute query
//            await parser.ExecuteGetAsync(query, OeRequestHeaders.Default, responceStream, CancellationToken.None);
//        }
//    }
//}
