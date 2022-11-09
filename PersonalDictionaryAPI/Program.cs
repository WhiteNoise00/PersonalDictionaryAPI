
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalDictionaryAPI.Data;
using PersonalDictionaryAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<APIDbContext>(option =>
    option.UseSqlServer(connection));
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
