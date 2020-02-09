using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Recipes",
    Author = "",
    Website = "",
    Version = "1.0.0",
    Description = "Provides Orchard Recipes.",
    Dependencies = new []
    {
        "SeedModules.Features",
        "OrchardCore.Scripting"
    },
    Category = "Infrastructure"
)]
