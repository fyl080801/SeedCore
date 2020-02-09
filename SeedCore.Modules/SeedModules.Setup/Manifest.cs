using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Setup",
    Author = "",
    Website = "",
    Version = "1.0.0",
    Description = "The setup module is creating the application's setup experience.",
    Dependencies = new[] { "SeedModules.Recipes" },
    Category = "Infrastructure"
)]
