using CSharpQuestions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddScoped<QuestionsService>();
builder.Services.AddScoped<TriangleAreaService>();
builder.Services.AddScoped<CheckLinksService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
