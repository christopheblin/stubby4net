﻿using NUnit.Framework;
using stubby.CLI;

namespace stubby.unit {

   [TestFixture]
   public class YamlParserTest {
      [Test]
      public void ShouldParseRequest_WithFile() {
         var endpoint = YamlParser.Parse("../../YAML/request-file.yaml")[0];
         Assert.AreEqual("path/to/file.txt", endpoint.Request.File);
      }

      [Test]
      public void ShouldParseRequest_WithHeaders() {
         var endpoint = YamlParser.Parse("../../YAML/request-headers.yaml")[0];

         Assert.AreEqual("text/xml", endpoint.Request.Headers["content-type"]);
         Assert.AreEqual("firefox(Mozilla)", endpoint.Request.Headers["user-agent"]);
      }

      [Test]
      public void ShouldParseRequest_WithMethod() {
         var endpoint = YamlParser.Parse("../../YAML/request-method.yaml")[0];
         Assert.AreEqual("DELETE", endpoint.Request.Method);
      }

      [Test]
      public void ShouldParseRequest_WithPostBody() {
         var endpoint = YamlParser.Parse("../../YAML/request-post.yaml")[0];
         Assert.AreEqual("post body!", endpoint.Request.Post);
      }

      [Test]
      public void ShouldParseRequest_WithQuery() {
         var endpoint = YamlParser.Parse("../../YAML/request-query.yaml")[0];

         Assert.AreEqual("tada", endpoint.Request.Query["first"]);
         Assert.AreEqual("voila", endpoint.Request.Query["second"]);
      }

      [Test]
      public void ShouldParseRequest_WithUrl() {
         var endpoint = YamlParser.Parse("../../YAML/hello-world.yaml")[0];
         Assert.AreEqual("/hello/world", endpoint.Request.Url);
      }

      [Test]
      public void ShouldParseResponse_WithBody() {
         var endpoint = YamlParser.Parse("../../YAML/response-body.yaml")[0];
         Assert.AreEqual("body contents!", endpoint.Response.Body);
      }

      [Test]
      public void ShouldParseResponse_WithFile() {
         var endpoint = YamlParser.Parse("../../YAML/response-file.yaml")[0];
         Assert.AreEqual("path/to/response/body", endpoint.Response.File);
      }

      [Test]
      public void ShouldParseResponse_WithHeaders() {
         var endpoint = YamlParser.Parse("../../YAML/response-headers.yaml")[0];

         Assert.AreEqual("application/json", endpoint.Response.Headers["content-type"]);
         Assert.AreEqual("application/json", endpoint.Response.Headers["accept"]);
      }

      [Test]
      public void ShouldParseResponse_WithLatency() {
         var endpoint = YamlParser.Parse("../../YAML/response-latency.yaml")[0];
         Assert.AreEqual(987654321, endpoint.Response.Latency);
      }

      [Test]
      public void ShouldParseResponse_WithStatus() {
         var endpoint = YamlParser.Parse("../../YAML/response-status.yaml")[0];
         Assert.AreEqual(204, endpoint.Response.Status);
      }

      [Test]
      public void ShouldParseMultipleEndpoints() {
         var endpoints = YamlParser.Parse("../../YAML/multiple.yaml");

         Assert.AreEqual(3, endpoints.Length);
      }
   }

}