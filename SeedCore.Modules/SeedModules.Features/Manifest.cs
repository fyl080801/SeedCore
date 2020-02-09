using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Features",
    Author = "",
    Website = "",
    Version = "1.0.0"
)]

[assembly: Feature(
    Id = "SeedModules.Features",
    Name = "Features",
    Description = "The Features module enables the administrator of the site to manage the installed modules as well as activate and de-activate features.",
    Dependencies = new string[0],
    Category = "Infrastructure"
)]
