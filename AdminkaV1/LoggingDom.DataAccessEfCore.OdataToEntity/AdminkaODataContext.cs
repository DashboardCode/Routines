using System;
using System.IO;
using System.Threading;
using Microsoft.OData.Edm;
using OdataToEntity;

using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.OdataToEntity
{
    /// <summary>
    /// OdataToEntity can't be referenced in Standard lib (only in .NET Framework)
    /// More information: https://github.com/voronov-maxim/OdataToEntity
    /// </summary>

    public class AdminkaODataContext
    {
        readonly OeParser parser;
        public AdminkaODataContext(string baseUri, IDbContextOptionsFactory optionsFactory, Action<string> verbose)
        {
            var dataAdapter = new AdminkaOeEfCoreDataAdapter(optionsFactory, verbose);
            //Build OData Edm Model
            EdmModel edmModel = dataAdapter.BuildEdmModel();
            //Create query parser
            this.parser = new OeParser(new Uri(baseUri /*"http://dummy"*/), edmModel);
        }

        public void AppendStream(Stream responceStream, Uri query)
        {
            //Query
            var uri = new Uri("http://dummy/Users?$select=Name");
            //The result of the query
            //Execute query
            parser.ExecuteGetAsync(query, OeRequestHeaders.JsonDefault, responceStream, CancellationToken.None);
        }
    }
}
