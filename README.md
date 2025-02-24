# Text to Speech POC

## Introduction
This project is a Proof of Concept (POC) for converting text to speech using Azure Cognitive Services. The application allows users to upload a file, extract text from it, and convert the extracted text to an audio file. The audio file can then be downloaded for further use.

## Sample Code
This repository contains sample code intended for demonstration purposes. It showcases how to integrate Azure Cognitive Services for text extraction and speech synthesis. The code is provided as-is and may require modifications to fit specific use cases or production environments.

## Getting Started

### Prerequisites
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure Subscription](https://azure.microsoft.com/en-us/free/)
- Azure Cognitive Services API keys for Document Intelligence, Azure Open AI, and Speech services.

### Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/cmw2/TextToSpeechPOC.git
    cd TextToSpeechPOC
    ```

1. Restore the dependencies:
    ```sh
    dotnet restore
    ```

1. Open the `appsettings.json` file and update the following sections with your Azure credentials:

    ```json
    {
      // ...existing code...
      "AzureDocumentIntelligence": {
        "Endpoint": "YOUR_AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT",
        "ApiKey": "YOUR_AZURE_DOCUMENT_INTELLIGENCE_API_KEY"
      },
      "AzureOpenAI": {
        "Endpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
        "ApiKey": "YOUR_AZURE_OPENAI_API_KEY",
        "ModelDeploymentName": "YOUR_MODEL_DEPLOYMENT_NAME",
      },
      "AzureSpeech": {
        "Region": "YOUR_AZURE_SPEECH_REGION",
        "ApiKey": "YOUR_AZURE_SPEECH_API_KEY",
        "VoiceName": "en-US-AriaNeural"
      }
      // ...existing code...
    }
    ```

    For security reasons, it is recommended to use dotnet user-secrets to store sensitive information such as API keys. Here is how you can set up dotnet user-secrets:
    1. Initialize user secrets in your project:
        ```sh
        dotnet user-secrets init
        ```
    1. Set the secrets using the following commands:
        ```sh
        dotnet user-secrets set "AzureDocumentIntelligence:ApiKey" "YOUR_DOCUMENT_INTELLIGENCE_API_KEY"
        ```
    In this manner the keys won't be checked into your source code.
1. Adjust the `SystemPrompt`, `UserPrompt`, `Temperature`, `MaxTokens`, and other settings in `appsettings.json` as needed for your specific use case.

### Running the Application
1. Build and run the application:
    ```sh
    dotnet run
    ```

2. Open your browser and navigate to `https://localhost:5001` to access the application.

### Usage
1. Upload a file using the "Upload a file" section.
2. Click "Process File" to extract text from the uploaded file.
3. Click "Extract Text" to use Azure OpenAI AI for text extraction.
4. Click "Generate Audio" to convert the extracted text to speech and download the audio file.

## Additional Information
- **Technologies Used**: ASP.NET Core, Blazor (Server), Azure Open AI, Azure Documnt Intelligence Service, Azure Speech Service

## Disclaimer
**This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment. THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED 'AS IS' WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.**
