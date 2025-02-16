﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;

namespace Azure.AI.FormRecognizer.DocumentAnalysis
{
    /// <summary>
    /// The client to use to connect to the Form Recognizer Azure Cognitive Service to analyze information from documents
    /// and images and extract it into structured data. It provides the ability to use prebuilt models to analyze receipts,
    /// business cards, invoices, to extract document content, and more. It's also possible to extract fields from custom
    /// documents with models built on custom document types.
    /// </summary>
    /// <remarks>
    /// Client is only available for <see cref="DocumentAnalysisClientOptions.ServiceVersion.V2021_09_30_preview"/> and higher.
    /// If you want to use a lower version, please use the <see cref="FormRecognizer.FormRecognizerClient"/>.
    /// </remarks>
    public class DocumentAnalysisClient
    {
        /// <summary>Provides communication with the Form Recognizer Azure Cognitive Service through its REST API.</summary>
        internal readonly DocumentAnalysisRestClient ServiceClient;

        /// <summary>Provides tools for exception creation in case of failure.</summary>
        internal readonly ClientDiagnostics Diagnostics;

        // TODO: update README link when we rename the README section to not mention the original FR Client?
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint to use for connecting to the Form Recognizer Azure Cognitive Service.</param>
        /// <param name="credential">A credential used to authenticate to an Azure Service.</param>
        /// <remarks>
        /// Both the <paramref name="endpoint"/> URI string and the <paramref name="credential"/> <c>string</c> key
        /// can be found in the Azure Portal.
        /// For more information see <see href="https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/README.md#authenticate-a-form-recognizer-client"> here</see>.
        /// </remarks>
        public DocumentAnalysisClient(Uri endpoint, AzureKeyCredential credential)
            : this(endpoint, credential, new DocumentAnalysisClientOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint to use for connecting to the Form Recognizer Azure Cognitive Service.</param>
        /// <param name="credential">A credential used to authenticate to an Azure Service.</param>
        /// <param name="options">A set of options to apply when configuring the client.</param>
        /// <remarks>
        /// Both the <paramref name="endpoint"/> URI string and the <paramref name="credential"/> <c>string</c> key
        /// can be found in the Azure Portal.
        /// For more information see <see href="https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/README.md#authenticate-a-form-recognizer-client"> here</see>.
        /// </remarks>
        public DocumentAnalysisClient(Uri endpoint, AzureKeyCredential credential, DocumentAnalysisClientOptions options)
        {
            Argument.AssertNotNull(endpoint, nameof(endpoint));
            Argument.AssertNotNull(credential, nameof(credential));

            options ??= new DocumentAnalysisClientOptions();

            Diagnostics = new ClientDiagnostics(options);
            var pipeline = HttpPipelineBuilder.Build(options, new AzureKeyCredentialPolicy(credential, Constants.AuthorizationHeader));
            ServiceClient = new DocumentAnalysisRestClient(Diagnostics, pipeline, endpoint.AbsoluteUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint to use for connecting to the Form Recognizer Azure Cognitive Service.</param>
        /// <param name="credential">A credential used to authenticate to an Azure Service.</param>
        /// <remarks>
        /// The <paramref name="endpoint"/> URI string can be found in the Azure Portal.
        /// For more information see <see href="https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/README.md#authenticate-a-form-recognizer-client"> here</see>.
        /// </remarks>
        public DocumentAnalysisClient(Uri endpoint, TokenCredential credential)
            : this(endpoint, credential, new DocumentAnalysisClientOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint to use for connecting to the Form Recognizer Azure Cognitive Service.</param>
        /// <param name="credential">A credential used to authenticate to an Azure Service.</param>
        /// <param name="options">A set of options to apply when configuring the client.</param>
        /// <remarks>
        /// The <paramref name="endpoint"/> URI string can be found in the Azure Portal.
        /// For more information see <see href="https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/README.md#authenticate-a-form-recognizer-client"> here</see>.
        /// </remarks>
        public DocumentAnalysisClient(Uri endpoint, TokenCredential credential, DocumentAnalysisClientOptions options)
        {
            Argument.AssertNotNull(endpoint, nameof(endpoint));
            Argument.AssertNotNull(credential, nameof(credential));

            options ??= new DocumentAnalysisClientOptions();

            Diagnostics = new ClientDiagnostics(options);
            var pipeline = HttpPipelineBuilder.Build(options, new BearerTokenAuthenticationPolicy(credential, Constants.DefaultCognitiveScope));
            ServiceClient = new DocumentAnalysisRestClient(Diagnostics, pipeline, endpoint.AbsoluteUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        protected DocumentAnalysisClient()
        {
        }

        // TODO: consider linking to document with prebuilt IDs when service goes public.
        /// <summary>
        /// Analyzes pages from one or more documents, using a model built with custom forms or one of the prebuilt
        /// models provided by the Form Recognizer service.
        /// </summary>
        /// <param name="modelId">
        /// The ID of the model to use for analyzing the input documents. When using a custom built model
        /// for analysis, this parameter must be the ID attributed to the model during its creation. When
        /// using one of the service's prebuilt models, one of the following values must be passed:
        /// <list type="bullet">
        /// <item>&quot;prebuilt-layout&quot;: extracts text, selection marks, tables, and layout information from documents.</item>
        /// <item>&quot;prebuilt-document&quot;: extracts text, selection marks, tables, layout information, entities, and key-value pairs from documents.</item>
        /// <item>&quot;prebuilt-businessCard&quot;: extracts text and key information from English business cards.</item>
        /// <item>&quot;prebuilt-idDocument&quot;: extracts text and key information from US driver licenses and international passports.</item>
        /// <item>&quot;prebuilt-invoice&quot;: extracts text, selection marks, tables, key-value pairs, and key information from invoices.</item>
        /// <item>&quot;prebuilt-receipt&quot;: extracts text and key information from English receipts.</item>
        /// </list>
        /// </param>
        /// <param name="document">The stream containing one or more documents to analyze.</param>
        /// <param name="analyzeDocumentOptions">
        /// A set of options available for configuring the analyze request. For example, specify the locale of the
        /// document, or which pages to analyze.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>
        /// An <see cref="AnalyzeDocumentOperation"/> to wait on this long-running operation. Its <see cref="AnalyzeDocumentOperation.Value"/> upon successful
        /// completion will contain analyzed pages from the input document.
        /// </returns>
        public virtual async Task<AnalyzeDocumentOperation> StartAnalyzeDocumentAsync(string modelId, Stream document, AnalyzeDocumentOptions analyzeDocumentOptions = default, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(modelId, nameof(modelId));
            Argument.AssertNotNull(document, nameof(document));

            analyzeDocumentOptions ??= new AnalyzeDocumentOptions();

            using DiagnosticScope scope = Diagnostics.CreateScope($"{nameof(DocumentAnalysisClient)}.{nameof(StartAnalyzeDocument)}");
            scope.Start();

            try
            {
                var response = await ServiceClient.DocumentAnalysisAnalyzeDocumentAsync(
                    modelId,
                    ContentType1.ApplicationOctetStream,
                    analyzeDocumentOptions.Pages.Count == 0 ? null : string.Join(",", analyzeDocumentOptions.Pages),
                    analyzeDocumentOptions.Locale,
                    Constants.DefaultStringIndexType,
                    document,
                    cancellationToken).ConfigureAwait(false);

                return new AnalyzeDocumentOperation(ServiceClient, Diagnostics, response.Headers.OperationLocation);
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary>
        /// Analyzes pages from one or more documents, using a model built with custom forms or one of the prebuilt
        /// models provided by the Form Recognizer service.
        /// </summary>
        /// <param name="modelId">
        /// The ID of the model to use for analyzing the input documents. When using a custom built model
        /// for analysis, this parameter must be the ID attributed to the model during its creation. When
        /// using one of the service's prebuilt models, one of the following values must be passed:
        /// <list type="bullet">
        /// <item>&quot;prebuilt-layout&quot;: extracts text, selection marks, tables, and layout information from documents.</item>
        /// <item>&quot;prebuilt-document&quot;: extracts text, selection marks, tables, layout information, entities, and key-value pairs from documents.</item>
        /// <item>&quot;prebuilt-businessCard&quot;: extracts text and key information from English business cards.</item>
        /// <item>&quot;prebuilt-idDocument&quot;: extracts text and key information from US driver licenses and international passports.</item>
        /// <item>&quot;prebuilt-invoice&quot;: extracts text, selection marks, tables, key-value pairs, and key information from invoices.</item>
        /// <item>&quot;prebuilt-receipt&quot;: extracts text and key information from English receipts.</item>
        /// </list>
        /// </param>
        /// <param name="document">The stream containing one or more documents to analyze.</param>
        /// <param name="analyzeDocumentOptions">
        /// A set of options available for configuring the analyze request. For example, specify the locale of the
        /// document, or which pages to analyze.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>
        /// An <see cref="AnalyzeDocumentOperation"/> to wait on this long-running operation. Its <see cref="AnalyzeDocumentOperation.Value"/> upon successful
        /// completion will contain analyzed pages from the input document.
        /// </returns>
        public virtual AnalyzeDocumentOperation StartAnalyzeDocument(string modelId, Stream document, AnalyzeDocumentOptions analyzeDocumentOptions = default, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(modelId, nameof(modelId));
            Argument.AssertNotNull(document, nameof(document));

            analyzeDocumentOptions ??= new AnalyzeDocumentOptions();

            using DiagnosticScope scope = Diagnostics.CreateScope($"{nameof(DocumentAnalysisClient)}.{nameof(StartAnalyzeDocument)}");
            scope.Start();

            try
            {
                var response = ServiceClient.DocumentAnalysisAnalyzeDocument(
                    modelId,
                    ContentType1.ApplicationOctetStream,
                    analyzeDocumentOptions.Pages.Count == 0 ? null : string.Join(",", analyzeDocumentOptions.Pages),
                    analyzeDocumentOptions.Locale,
                    Constants.DefaultStringIndexType,
                    document,
                    cancellationToken);

                return new AnalyzeDocumentOperation(ServiceClient, Diagnostics, response.Headers.OperationLocation);
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary>
        /// Analyzes pages from one or more documents, using a model built with custom forms or one of the prebuilt
        /// models provided by the Form Recognizer service.
        /// </summary>
        /// <param name="modelId">
        /// The ID of the model to use for analyzing the input documents. When using a custom built model
        /// for analysis, this parameter must be the ID attributed to the model during its creation. When
        /// using one of the service's prebuilt models, one of the following values must be passed:
        /// <list type="bullet">
        /// <item>&quot;prebuilt-layout&quot;: extracts text, selection marks, tables, and layout information from documents.</item>
        /// <item>&quot;prebuilt-document&quot;: extracts text, selection marks, tables, layout information, entities, and key-value pairs from documents.</item>
        /// <item>&quot;prebuilt-businessCard&quot;: extracts text and key information from English business cards.</item>
        /// <item>&quot;prebuilt-idDocument&quot;: extracts text and key information from US driver licenses and international passports.</item>
        /// <item>&quot;prebuilt-invoice&quot;: extracts text, selection marks, tables, key-value pairs, and key information from invoices.</item>
        /// <item>&quot;prebuilt-receipt&quot;: extracts text and key information from English receipts.</item>
        /// </list>
        /// </param>
        /// <param name="documentUri">The absolute URI of the remote file to analyze documents from.</param>
        /// <param name="analyzeDocumentOptions">
        /// A set of options available for configuring the analyze request. For example, specify the locale of the
        /// document, or which pages to analyze.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>
        /// An <see cref="AnalyzeDocumentOperation"/> to wait on this long-running operation. Its <see cref="AnalyzeDocumentOperation.Value"/> upon successful
        /// completion will contain analyzed pages from the input document.
        /// </returns>
        public virtual async Task<AnalyzeDocumentOperation> StartAnalyzeDocumentFromUriAsync(string modelId, Uri documentUri, AnalyzeDocumentOptions analyzeDocumentOptions = default, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(modelId, nameof(modelId));
            Argument.AssertNotNull(documentUri, nameof(documentUri));

            analyzeDocumentOptions ??= new AnalyzeDocumentOptions();

            using DiagnosticScope scope = Diagnostics.CreateScope($"{nameof(DocumentAnalysisClient)}.{nameof(StartAnalyzeDocumentFromUri)}");
            scope.Start();

            try
            {
                var request = new AnalyzeDocumentRequest() { UrlSource = documentUri.AbsoluteUri };
                var response = await ServiceClient.DocumentAnalysisAnalyzeDocumentAsync(
                    modelId,
                    analyzeDocumentOptions.Pages.Count == 0 ? null : string.Join(",", analyzeDocumentOptions.Pages),
                    analyzeDocumentOptions.Locale,
                    Constants.DefaultStringIndexType,
                    request,
                    cancellationToken).ConfigureAwait(false);

                return new AnalyzeDocumentOperation(ServiceClient, Diagnostics, response.Headers.OperationLocation);
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        /// <summary>
        /// Analyzes pages from one or more documents, using a model built with custom forms or one of the prebuilt
        /// models provided by the Form Recognizer service.
        /// </summary>
        /// <param name="modelId">
        /// The ID of the model to use for analyzing the input documents. When using a custom built model
        /// for analysis, this parameter must be the ID attributed to the model during its creation. When
        /// using one of the service's prebuilt models, one of the following values must be passed:
        /// <list type="bullet">
        /// <item>&quot;prebuilt-layout&quot;: extracts text, selection marks, tables, and layout information from documents.</item>
        /// <item>&quot;prebuilt-document&quot;: extracts text, selection marks, tables, layout information, entities, and key-value pairs from documents.</item>
        /// <item>&quot;prebuilt-businessCard&quot;: extracts text and key information from English business cards.</item>
        /// <item>&quot;prebuilt-idDocument&quot;: extracts text and key information from US driver licenses and international passports.</item>
        /// <item>&quot;prebuilt-invoice&quot;: extracts text, selection marks, tables, key-value pairs, and key information from invoices.</item>
        /// <item>&quot;prebuilt-receipt&quot;: extracts text and key information from English receipts.</item>
        /// </list>
        /// </param>
        /// <param name="documentUri">The absolute URI of the remote file to analyze documents from.</param>
        /// <param name="analyzeDocumentOptions">
        /// A set of options available for configuring the analyze request. For example, specify the locale of the
        /// document, or which pages to analyze.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifetime.</param>
        /// <returns>
        /// An <see cref="AnalyzeDocumentOperation"/> to wait on this long-running operation. Its <see cref="AnalyzeDocumentOperation.Value"/> upon successful
        /// completion will contain analyzed pages from the input document.
        /// </returns>
        public virtual AnalyzeDocumentOperation StartAnalyzeDocumentFromUri(string modelId, Uri documentUri, AnalyzeDocumentOptions analyzeDocumentOptions = default, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(modelId, nameof(modelId));
            Argument.AssertNotNull(documentUri, nameof(documentUri));

            analyzeDocumentOptions ??= new AnalyzeDocumentOptions();

            using DiagnosticScope scope = Diagnostics.CreateScope($"{nameof(DocumentAnalysisClient)}.{nameof(StartAnalyzeDocumentFromUri)}");
            scope.Start();

            try
            {
                var request = new AnalyzeDocumentRequest() { UrlSource = documentUri.AbsoluteUri };
                var response = ServiceClient.DocumentAnalysisAnalyzeDocument(
                    modelId,
                    analyzeDocumentOptions.Pages.Count == 0 ? null : string.Join(",", analyzeDocumentOptions.Pages),
                    analyzeDocumentOptions.Locale,
                    Constants.DefaultStringIndexType,
                    request,
                    cancellationToken);

                return new AnalyzeDocumentOperation(ServiceClient, Diagnostics, response.Headers.OperationLocation);
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }
    }
}
