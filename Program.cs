using Microsoft.AspNetCore.Mvc;
using IMCCalculator.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AcessoTotal",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
    }
);

var app = builder.Build();

// Usuarios
app.MapPost("/api/usuarios/cadastrar", ([FromBody] Usuario usuario, [FromServices] AppDataContext context) =>
{
    var erros = new List<ValidationResult>();
    if (!Validator.TryValidateObject(usuario, new ValidationContext(usuario), erros, true))
    {
        return Results.BadRequest(erros);
    }

    var usuarioExistente = context.Usuarios.FirstOrDefault(u => u.Nome == usuario.Nome);
    if (usuarioExistente != null)
    {
        return Results.BadRequest("Já existe um usuário com este nome.");
    }

    context.Usuarios.Add(usuario);
    context.SaveChanges();
    return Results.Created($"Usuario {usuario.Nome} cadastrado com sucesso", usuario);
});

app.MapGet("/api/usuarios/listar", ([FromServices] AppDataContext context) =>
{
    var usuarios = context.Usuarios.ToList();
    return usuarios.Count > 0 ? Results.Ok(usuarios) : Results.NotFound("Não há usuários cadastrados!");
});

app.MapGet("/api/usuarios/buscar/{id}", ([FromRoute] Guid id, [FromServices] AppDataContext context) =>
{
    var usuario = context.Usuarios.Find(id.ToString());
    return usuario != null ? Results.Ok(usuario) : Results.NotFound("Usuário não encontrado!");
});

app.MapDelete("/api/usuarios/deletar/{id}", ([FromRoute] string id, [FromServices] AppDataContext context) =>
{
    var usuario = context.Usuarios.Find(id);
    if (usuario == null)
        return Results.NotFound("Usuário não encontrado!");

    context.Usuarios.Remove(usuario);
    context.SaveChanges();
    return Results.Ok($"Usuário {usuario.Nome} deletado com sucesso!");
});

app.MapPut("/api/usuarios/alterar/{id}", ([FromRoute] string id, [FromBody] Usuario usuarioAtualizado, [FromServices] AppDataContext context) =>
{
    var usuario = context.Usuarios.Find(id);
    if (usuario == null)
        return Results.NotFound("Usuário não encontrado.");

    var usuarioExistente = context.Usuarios.FirstOrDefault(u => u.Nome == usuarioAtualizado.Nome && u.Id != id);
    if (usuarioExistente != null)
    {
        return Results.BadRequest("Já existe um usuário com este nome.");
    }

    usuario.Nome = usuarioAtualizado.Nome;
    usuario.DataDeNascimento = usuarioAtualizado.DataDeNascimento;

    context.SaveChanges();
    return Results.Ok($"Usuário {usuario.Nome} alterado com sucesso.");
});

// IMC
app.MapPost("/api/imcs/cadastrar/{usuarioId}", ([FromBody] Imc imc, string usuarioId, [FromServices] AppDataContext context) =>
{
    var erros = new List<ValidationResult>();
    if (!Validator.TryValidateObject(imc, new ValidationContext(imc), erros, true))
    {
        return Results.BadRequest(erros);
    }

    var usuario = context.Usuarios.Find(usuarioId);
    if (usuario == null)
    {
        return Results.NotFound($"Usuário com ID {usuarioId} não encontrado.");
    }

    imc.UsuarioId = usuarioId;
    imc.CalcularResultado();

    context.Imcs.Add(imc);
    context.SaveChanges();
    return Results.Created($"IMC cadastrado com sucesso", imc);
});

app.MapGet("/api/imcs/listar", ([FromServices] AppDataContext context) =>
{
    var imcs = context.Imcs.ToList();
    return imcs.Count > 0 ? Results.Ok(imcs) : Results.NotFound("Não há registros de IMC cadastrados.");
});

app.MapGet("/api/imcs/buscar/{id}", ([FromRoute] string id, [FromServices] AppDataContext context) =>
{
    var imc = context.Imcs.Find(id);
    return imc != null ? Results.Ok(imc) : Results.NotFound("Registro de IMC não encontrado.");
});

app.MapDelete("/api/imcs/deletar/{id}", ([FromRoute] string id, [FromServices] AppDataContext context) =>
{
    var imc = context.Imcs.Find(id);
    if (imc == null)
        return Results.NotFound("Registro de IMC não encontrado.");

    context.Imcs.Remove(imc);
    context.SaveChanges();
    return Results.Ok($"Registro de IMC deletado com sucesso.");
});

app.MapPut("/api/imcs/alterar/{id}", ([FromRoute] string id, [FromBody] Imc imcAtualizado, [FromServices] AppDataContext context) =>
{
    var imc = context.Imcs.Find(id);
    if (imc == null)
        return Results.NotFound("Registro de IMC não encontrado.");

    imc.Altura = imcAtualizado.Altura;
    imc.Peso = imcAtualizado.Peso;
    imc.CalcularResultado();

    context.SaveChanges();
    return Results.Ok($"Registro de IMC alterado com sucesso.");
});

app.UseCors("AcessoTotal");
app.Run();
