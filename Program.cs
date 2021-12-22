var builder = WebApplication.CreateBuilder(args);

// Register your services
RegisterServices(builder.Services);

var app = builder.Build();

// App configurations
ConfigureApp(app);

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.Configure<StudentDatabaseSettings>(builder.Configuration.GetSection("StudentDatabaseSettings"));
    services.AddSingleton<StudentService>();

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Contacts API",
            Description = "Storing and sharing contacts",
            Version = "v1"
        });
    });
}

void ConfigureApp(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/api/student", async (StudentService service)
        => await service.GetAll());

    app.MapGet("/api/student/{id}", async (StudentService service, string id)
        => await service.Get(id));

    app.MapPost("/api/student", async (StudentService service, Student student) =>
    {
        await service.Create(student);
        return Results.Created($"/student/{student._id}", student);
    });

    app.MapPut("/api/student/{id}", async (StudentService service, string id, Student updateStudent) =>
    {
        var student = await service.Get(id);

        if (student is null)
            return Results.NotFound();

        updateStudent._id = student._id;

        await service.Update(id, updateStudent);

        return Results.NoContent();
    });

    app.MapDelete("/api/student/{id}", async (StudentService service, string id) =>
    {
        var student = await service.Get(id);

        if (student is null) return Results.NotFound();

        await service.Delete(id);

        return Results.NoContent();
    });
}