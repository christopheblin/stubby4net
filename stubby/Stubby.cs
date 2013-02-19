using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using stubby.CLI;
using stubby.Domain;
using stubby.Exceptions;
using stubby.Portals;

namespace stubby {

   public class Stubby : IDisposable {
      private readonly Admin _admin;
      private readonly IArguments _arguments;
      private readonly EndpointDb _endpointDb;
      private readonly Stubs _stubs;

      public Stubby(IArguments arguments) {
         _arguments = arguments ?? new Arguments {Mute = true};
         _endpointDb = new EndpointDb();
         _admin = new Admin(_endpointDb);
         _stubs = new Stubs(_endpointDb);

         Out.Mute = _arguments.Mute;
         LoadEndpoints();
      }

      public void Dispose() {
         _admin.Dispose();
         _stubs.Dispose();
      }

      /// <summary>
      /// Start stubby's services
      /// </summary>
      public void Start() {
         StartPortals();
      }

      /// <summary>
      /// Stop stubby's services
      /// </summary>
      public void Stop() {
         _admin.Stop();
         _stubs.Stop();
      }

      /// <summary>
      /// Get a listing of all of stubby's configured endpoints
      /// </summary>
      public IList<Endpoint> GetAll() {
         return _endpointDb.Fetch();
      }

      /// <summary>
      /// Get an endpoint back by id
      /// </summary>
      public Endpoint Get(uint id) {
         return _endpointDb.Fetch(id);
      }

      /// <summary>
      /// Swap out the configuration of one of the endpoints
      /// </summary>
      /// <param name="id">The id of the endpoint to replace</param>
      /// <param name="endpoint">The new endpoint data</param>
      /// <returns>True if the operation succeeded</returns>
      public bool Replace(uint id, Endpoint endpoint) {
         return _endpointDb.Replace(id, endpoint);
      }

      /// <summary>
      /// Swap out the configuration of several endpoints
      /// </summary>
      /// <param name="endpoints">An &lt;id, endpoint&gt; Map of endpoints to swap out</param>
      /// <returns>True if all given endpoints were replaced</returns>
      public bool Replace(IEnumerable<KeyValuePair<uint, Endpoint>> endpoints) {
         return _endpointDb.Replace(endpoints);
      }

      /// <summary>
      /// Remove an endpoint by id
      /// </summary>
      /// <returns>True if the operation succeeded</returns>
      public bool Delete(uint id) {
         return _endpointDb.Delete(id);
      }

      /// <summary>
      /// Remove all configured endpoints from stubby
      /// </summary>
      public void DeleteAll() {
         _endpointDb.Delete();
      }

      /// <summary>
      /// Add a new endpoint configuration
      /// </summary>
      /// <param name="endpoint">The new endpoint data</param>
      /// <param name="id">The new generated id, for use with Replace/Delete</param>
      /// <returns>True if the operation succeeded</returns>
      public bool Add(Endpoint endpoint, out uint id) {
         return _endpointDb.Insert(endpoint, out id);
      }

      /// <summary>
      /// Add many new endpoint configurations
      /// </summary>
      /// <param name="endpoints">The new endpoints data</param>
      /// <param name="ids">The new generated ids of the inserted endpoints</param>
      /// <returns>True if all endpoints were added</returns>
      public bool Add(IEnumerable<Endpoint> endpoints, out IList<uint> ids) {
         return _endpointDb.Insert(endpoints, out ids);
      }

      private void LoadEndpoints() {
         IList<Endpoint> endpoints = new List<Endpoint>();

         try {
            endpoints = YamlParser.FromFile(_arguments.Data);
         } catch (FileNotFoundException) {
            Out.Warn("File '" + _arguments.Data + "' couldn't be found. Ignoring...");
         } catch (YamlException ex) {
            Out.Error(ex.Message);
            throw new EndpointParsingException("Could not parse endpoints due to YAML errors.", ex);
         }

         _endpointDb.Insert(endpoints);
      }

      private void StartPortals() {
         Out.Linefeed();
         _admin.Start(_arguments.Location, _arguments.Admin);
         _stubs.Start(_arguments.Location, _arguments.Stubs);
         Out.Linefeed();
      }
   }

}