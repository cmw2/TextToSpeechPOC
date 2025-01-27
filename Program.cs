using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TextToSpeechPOC.Data;
using TextToSpeechPOC.Options;

var builder = WebApplication.CreateBuilder(args);

// Add this line to configure the options
builder.Services.Configure<AzureDocumentIntelligenceOptions>(builder.Configuration.GetSection("AzureDocumentIntelligence"));
builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));
builder.Services.Configure<AzureSpeechOptions>(builder.Configuration.GetSection("AzureSpeech"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddSingleton<AzureDocumentIntelligenceService>();
builder.Services.AddSingleton<AzureOpenAIService>();
builder.Services.AddSingleton<AzureSpeechService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers(); 
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
