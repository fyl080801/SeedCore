using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Commands;
using OrchardCore.Users.Services;
using SeedModules.Account.Domain;

namespace SeedModules.Account.Commands
{
    public class UserCommands : DefaultCommandHandler
    {
        private readonly IUserService _userService;

        public UserCommands(
            IUserService userService,
            IStringLocalizer<UserCommands> localizer)
            : base(localizer)
        {
            _userService = userService;
        }

        [OrchardSwitch]
        public string UserName { get; set; }

        [OrchardSwitch]
        public string Password { get; set; }

        [OrchardSwitch]
        public string Email { get; set; }

        // [OrchardSwitch]
        // public string Roles { get; set; }

        [CommandName("createUser")]
        [CommandHelp("createUser /UserName:<username> /Password:<password> /Email:<email>\r\n\t" + "Creates a new User")]
        [OrchardSwitches("UserName,Password,Email,Roles")]
        public async Task CreateUserAsync()
        {
            // var roleNames = (Roles ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var valid = true;

            // 先不能加入角色
            await _userService.CreateUserAsync(new User
            {
                UserName = UserName,
                Email = Email,
                EmailConfirmed = true
            }, Password, (key, message) =>
            {
                valid = false;
                Context.Output.WriteLine(message);
            });

            if (valid)
            {
                Context.Output.WriteLine(S["User created successfully"]);
            }
        }
    }
}
