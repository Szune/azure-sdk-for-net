# Azure Cognitive Services Form Recognizer client library for .NET
Azure Cognitive Services Form Recognizer is a cloud service that uses machine learning to recognize form fields, text, and tables in form documents.  It includes the following capabilities:

- Recognize Custom Forms - Recognize and extract form fields and other content from your custom forms, using models you trained with your own form types.
- Recognize Form Content - Recognize and extract tables, lines, words, and selection marks like radio buttons and check boxes in form documents, without the need to train a model.
- Recognize Prebuilt models - Recognize data using the following prebuilt models:
  - Receipts - Recognize and extract common fields from receipts, using a pre-trained receipt model.
  - Business Cards - Recognize and extract common fields from business cards, using a pre-trained business cards model.
  - Invoices - Recognize and extract common fields from invoices, using a pre-trained invoice model.
  - Identity Documents - Recognize and extract common fields from identity documents like passports or driver's licenses, using a pre-trained identity documents model.

[Source code][formreco_client_src] | [Package (NuGet)][formreco_nuget_package] | [API reference documentation][formreco_refdocs] | [Product documentation][formreco_docs] | [Samples][formreco_samples]

## Getting started

### Install the package
Install the Azure Form Recognizer client library for .NET with [NuGet][nuget]:

```dotnetcli
dotnet add package Azure.AI.FormRecognizer
``` 

> Note: This version of the client library defaults to the `v2.1` version of the service.

This table shows the relationship between SDK versions and supported API versions of the service:

|SDK version|Supported API version of service
|-|-
|3.0.0 | 2.0
|3.0.1 | 2.0
|3.1.X | 2.0, 2.1

### Prerequisites
* An [Azure subscription][azure_sub].
* An existing Cognitive Services or Form Recognizer resource.

#### Create a Cognitive Services or Form Recognizer resource
Form Recognizer supports both [multi-service and single-service access][cognitive_resource_portal]. Create a Cognitive Services resource if you plan to access multiple cognitive services under a single endpoint/key. For Form Recognizer access only, create a Form Recognizer resource. Please note that you will need a single-service resource if you intend to use [Azure Active Directory authentication](#create-formrecognizerclient-with-azure-active-directory-credential).

You can create either resource using: 

**Option 1:** [Azure Portal][cognitive_resource_portal].

**Option 2:** [Azure CLI][cognitive_resource_cli]. 


Below is an example of how you can create a Form Recognizer resource using the CLI:

```PowerShell
# Create a new resource group to hold the form recognizer resource
# if using an existing resource group, skip this step
az group create --name <your-resource-name> --location <location>
```

```PowerShell
# Create form recognizer 
az cognitiveservices account create \
    --name <your-resource-name> \
    --resource-group <your-resource-group-name> \
    --kind FormRecognizer \
    --sku <sku> \
    --location <location> \
    --yes
```
For more information about creating the resource or how to get the location and sku information see [here][cognitive_resource_cli].

### Authenticate the client
In order to interact with the Form Recognizer service, you'll need to create an instance of the [`FormRecognizerClient`][form_recognizer_client_class] class.  You will need an **endpoint** and an **API key** to instantiate a client object.  

#### Get the endpoint

You can obtain the endpoint from the resource information in the [Azure Portal][azure_portal].

Either a regional endpoint or a custom subdomain can be used for authentication. They are formatted as follows:

```
Regional endpoint: https://<region>.api.cognitive.microsoft.com/
Custom subdomain: https://<resource-name>.cognitiveservices.azure.com/
```

A regional endpoint is the same for every resource in a region. A complete list of supported regional endpoints can be consulted [here][regional_endpoints]. Please note that regional endpoints do not support AAD authentication.

A custom subdomain, on the other hand, is a name that is unique to the Form Recognizer resource. They can only be used by [single-service resources][cognitive_resource_portal].

#### Get API Key

You can obtain the API key from the resource information in the [Azure Portal][azure_portal].

Alternatively, you can use the [Azure CLI][azure_cli] snippet below to get the API key from the Form Recognizer resource.

```PowerShell
az cognitiveservices account keys list --resource-group <your-resource-group-name> --name <your-resource-name>
```

#### Create FormRecognizerClient with AzureKeyCredential
Once you have the value for the API key, create an `AzureKeyCredential`.  With the endpoint and key credential, you can create the [`FormRecognizerClient`][form_recognizer_client_class]:

```C# Snippet:CreateFormRecognizerClient
string endpoint = "<endpoint>";
string apiKey = "<apiKey>";
var credential = new AzureKeyCredential(apiKey);
var client = new FormRecognizerClient(new Uri(endpoint), credential);
```

#### Create FormRecognizerClient with Azure Active Directory Credential

`AzureKeyCredential` authentication is used in the examples in this getting started guide, but you can also authenticate with Azure Active Directory using the [Azure Identity library][azure_identity]. Note that regional endpoints do not support AAD authentication. Create a [custom subdomain][custom_subdomain] for your resource in order to use this type of authentication.

To use the [DefaultAzureCredential][DefaultAzureCredential] provider shown below, or other credential providers provided with the Azure SDK, please install the `Azure.Identity` package:

```PowerShell
Install-Package Azure.Identity
```

You will also need to [register a new AAD application][register_aad_app] and [grant access][aad_grant_access] to Form Recognizer by assigning the `"Cognitive Services User"` role to your service principal.

Set the values of the client ID, tenant ID, and client secret of the AAD application as environment variables: AZURE_CLIENT_ID, AZURE_TENANT_ID, AZURE_CLIENT_SECRET.

```C# Snippet:CreateFormRecognizerClientTokenCredential
string endpoint = "<endpoint>";
var client = new FormRecognizerClient(new Uri(endpoint), new DefaultAzureCredential());
```

## Key concepts

### FormRecognizerClient

`FormRecognizerClient` provides operations for:

- Recognizing form fields and content, using custom models trained to recognize your custom forms.  These values are returned in a collection of `RecognizedForm` objects. See example [Recognize Custom Forms](#recognize-custom-forms).
- Recognizing form content, including tables, lines, words, and selection marks like radio buttons and check boxes without the need to train a model.  Form content is returned in a collection of `FormPage` objects. See example [Recognize Content](#recognize-content).
- Recognizing common fields from the following form types using prebuilt models. These fields and meta-data are returned in a collection of `RecognizedForm` objects.
Supported prebuilt models:
  - Receipts
  - Business cards
  - Invoices
  - Identity Documents

### FormTrainingClient

`FormTrainingClient` provides operations for:

- Training custom models to recognize all fields and values found in your custom forms.  A `CustomFormModel` is returned indicating the form types the model will recognize, and the fields it will extract for each form type.
- Training custom models to recognize specific fields and values you specify by labeling your custom forms.  A `CustomFormModel` is returned indicating the fields the model will extract, as well as the estimated accuracy for each field.
- Managing models created in your account.
- Copying a custom model from one Form Recognizer resource to another.
- Creating a composed model from a collection of existing models trained with labels.

See examples for [Train a Model](#train-a-model) and [Manage Custom Models](#manage-custom-models).

Please note that models can also be trained using a graphical user interface such as the [Form Recognizer Labeling Tool][labeling_tool].

### Long-Running Operations

Because analyzing and training form documents takes time, these operations are implemented as [**long-running operations**][dotnet_lro_guidelines].  Long-running operations consist of an initial request sent to the service to start an operation, followed by polling the service at intervals to determine whether the operation has completed or failed, and if it has succeeded, to get the result.

For long running operations in the Azure SDK, the client exposes a `Start<operation-name>` method that returns an `Operation<T>`.  You can use the extension method `WaitForCompletionAsync()` to wait for the operation to complete and obtain its result.  A sample code snippet is provided to illustrate using long-running operations [below](#recognize-content).

### Thread safety
We guarantee that all client instance methods are thread-safe and independent of each other ([guideline](https://azure.github.io/azure-sdk/dotnet_introduction.html#dotnet-service-methods-thread-safety)). This ensures that the recommendation of reusing client instances is always safe, even across threads.

### Additional concepts
<!-- CLIENT COMMON BAR -->
[Client options](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/README.md#configuring-service-clients-using-clientoptions) |
[Accessing the response](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/README.md#accessing-http-response-details-using-responset) |
[Handling failures](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/README.md#reporting-errors-requestfailedexception) |
[Diagnostics](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/samples/Diagnostics.md) |
[Mocking](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/README.md#mocking) |
[Client lifetime](https://devblogs.microsoft.com/azure-sdk/lifetime-management-and-thread-safety-guarantees-of-azure-sdk-net-clients/)
<!-- CLIENT COMMON BAR -->

## Examples
The following section provides several code snippets illustrating common patterns used in the Form Recognizer .NET API. Most of the snippets below make use of asynchronous service calls, but keep in mind that the Azure.AI.FormRecognizer package supports both synchronous and asynchronous APIs.

### Async examples
* [Recognize Content](#recognize-content)
* [Recognize Custom Forms](#recognize-custom-forms)
* [Use Prebuilt Models](#use-prebuilt-models)
* [Train a Model](#train-a-model)
* [Manage Custom Models](#manage-custom-models)

### Sync examples
* [Manage Custom Models Synchronously](#manage-custom-models-synchronously)

### Recognize Content
Recognize text, tables, and selection marks like radio buttons and check boxes data, along with their bounding box coordinates, from documents.

For more information and samples see [here][recognize_content].

### Recognize Custom Forms
Recognize and extract form fields and other content from your custom forms, using models you train with your own form types.

For more information and samples see [here][recognize_custom_forms].

### Use Prebuilt Models
Extract fields from certain types of common forms using prebuilt models provided by the Form Recognizer service. Supported prebuilt models are:
- Sales receipts. See fields found on a receipt [here][service_recognize_receipt_fields].
- Business cards. See fields found on a business card [here][service_recognize_business_cards_fields].
- Invoices. See fields found on an invoice [here][service_recognize_invoices_fields].
- Identity documents. See fields found on an identity document [here][service_recognize_identity_documents_fields].

For example, to extract fields from a sales receipt, use the prebuilt Receipt model provided by the `StartRecognizeReceiptsAsync` method:

For more information and samples using prebuilt models see:
- [Receipts sample][recognize_receipts].
- [Business Cards sample][recognize_business_cards].
- [Invoices][recognize_invoices].
- [Identity Documents][recognize_identity_documents].

### Train a Model
Train a machine-learned model on your own form types. The resulting model will be able to recognize values from the types of forms it was trained on.

For more information and samples see [here][train_a_model].

### Manage Custom Models
Manage the custom models stored in your account.

For more information and samples see [here][manage_custom_models].

### Manage Custom Models Synchronously
Manage the custom models stored in your account with a synchronous API. Note that we are still making an asynchronous call to `WaitForCompletionAsync` for training, since this method does not have a synchronous counterpart. For more information on long-running operations, see [Long-Running Operations](#long-running-operations).

## Troubleshooting

### General
When you interact with the Cognitive Services Form Recognizer client library using the .NET SDK, errors returned by the service will result in a `RequestFailedException` with the same HTTP status code returned by the [REST API][formreco_rest_api] request.

For example, if you submit a receipt image with an invalid `Uri`, a `400` error is returned, indicating "Bad Request".

### Setting up console logging
The simplest way to see the logs is to enable the console logging.
To create an Azure SDK log listener that outputs messages to console use the AzureEventSourceListener.CreateConsoleLogger method.

```C#
// Setup a listener to monitor logged events.
using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();
```

To learn more about other logging mechanisms see [Diagnostics Samples][logging].

## Next steps

Samples showing how to use the Cognitive Services Form Recognizer library are available in this GitHub repository. Samples are provided for each main functional area:

- [Recognize form content][recognize_content]
- [Recognize custom forms][recognize_custom_forms]
- [Recognize receipts][recognize_receipts]
- [Recognize business cards][recognize_business_cards]
- [Recognize invoices][recognize_invoices]
- [Recognize identity documents][recognize_identity_documents]
- [Train a model][train_a_model]
- [Manage custom models][manage_custom_models]
- [Copy a custom model between Form Recognizer resources][copy_custom_models]
- [Create composed model from a collection of models trained with labels][composed_model]

> Note that these samples use SDK `V4.0.0-beta.X`. For lower versions of the SDK, please see [Form Recognizer Samples for V3.1.X][formrecov3_samples].

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit [cla.microsoft.com][cla].

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct][code_of_conduct]. For more information see the [Code of Conduct FAQ][coc_faq] or contact [opencode@microsoft.com][coc_contact] with any additional questions or comments.

![Impressions](https://azure-sdk-impressions.azurewebsites.net/api/impressions/azure-sdk-for-net%2Fsdk%2Fformrecognizer%2FAzure.AI.FormRecognizer%2FREADME.png)


<!-- LINKS -->
[formreco_client_src]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/src
[formreco_docs]: https://docs.microsoft.com/azure/cognitive-services/form-recognizer/
[formreco_refdocs]: https://aka.ms/azsdk/net/docs/ref/formrecognizer
[formreco_nuget_package]: https://www.nuget.org/packages/Azure.AI.FormRecognizer
[formreco_samples]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/README.md
[formrecov3_samples]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/README.md
[formreco_rest_api]: https://westus2.dev.cognitive.microsoft.com/docs/services/form-recognizer-api-v2-1/operations/AnalyzeBusinessCardAsync
[cognitive_resource]: https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account


[form_recognizer_client_class]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/src/FormRecognizerClient/FormRecognizerClient.cs
[azure_identity]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/identity/Azure.Identity
[cognitive_auth]: https://docs.microsoft.com/azure/cognitive-services/authentication
[register_aad_app]: https://docs.microsoft.com/azure/cognitive-services/authentication#assign-a-role-to-a-service-principal
[aad_grant_access]: https://docs.microsoft.com/azure/cognitive-services/authentication#assign-a-role-to-a-service-principal
[custom_subdomain]: https://docs.microsoft.com/azure/cognitive-services/authentication#create-a-resource-with-a-custom-subdomain
[DefaultAzureCredential]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/identity/Azure.Identity/README.md
[cognitive_resource_portal]: https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account
[cognitive_resource_cli]: https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account-cli
[regional_endpoints]: https://docs.microsoft.com/azure/cognitive-services/cognitive-services-custom-subdomains#is-there-a-list-of-regional-endpoints


[labeling_tool]: https://aka.ms/azsdk/formrecognizer/labelingtool
[service_recognize_receipt_fields]: https://aka.ms/formrecognizer/receiptfields
[service_recognize_business_cards_fields]: https://aka.ms/formrecognizer/businesscardfields
[service_recognize_invoices_fields]: https://aka.ms/formrecognizer/invoicefields
[service_recognize_identity_documents_fields]: https://aka.ms/formrecognizer/iddocumentfields
[dotnet_lro_guidelines]: https://azure.github.io/azure-sdk/dotnet_introduction.html#dotnet-longrunning

[logging]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/core/Azure.Core/samples/Diagnostics.md

[recognize_content]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample1_RecognizeFormContent.md
[recognize_custom_forms]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample2_RecognizeCustomForms.md
[recognize_receipts]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample3_RecognizeReceipts.md
[recognize_business_cards]: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample9_RecognizeBusinessCards.md
[recognize_invoices]: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample10_RecognizeInvoices.md
[recognize_identity_documents]: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample11_RecognizeIdentityDocuments.md
[train_a_model]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample5_TrainModel.md
[manage_custom_models]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample6_ManageCustomModels.md
[copy_custom_models]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample7_CopyCustomModel.md
[composed_model]: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/V3/Sample8_ModelCompose.md

[azure_cli]: https://docs.microsoft.com/cli/azure
[azure_sub]: https://azure.microsoft.com/free/dotnet/
[nuget]: https://www.nuget.org/
[azure_portal]: https://portal.azure.com

[cla]: https://cla.microsoft.com
[code_of_conduct]: https://opensource.microsoft.com/codeofconduct/
[coc_faq]: https://opensource.microsoft.com/codeofconduct/faq/
[coc_contact]: mailto:opencode@microsoft.com
