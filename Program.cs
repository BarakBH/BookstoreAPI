using BookstoreApi.Services;

var builder = WebApplication.CreateBuilder(args);

// no options binding needed now
builder.Services.AddSingleton<IBookRepository, XmlBookRepository>();
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();
