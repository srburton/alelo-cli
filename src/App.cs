using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.CommandLine.Invocation;

namespace Alelo.Console
{
    internal static class App
    {
        private static async Task<int> Main(string[] args)
        {
            var aleloHome = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALELO_HOME"))
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".alelo")
                : Environment.GetEnvironmentVariable("ALELO_HOME");

            var aleloDefaultProfile = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALELO_DEFAULT_PROFILE"))
                ? GetProfilesNames(false).FirstOrDefault()
                : Environment.GetEnvironmentVariable("ALELO_DEFAULT_PROFILE");

            var aleloDefaultCard = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALELO_DEFAULT_CARD"))
                ? string.Empty
                : Environment.GetEnvironmentVariable("ALELO_DEFAULT_CARD");

            if (!Directory.Exists(aleloHome))
                Directory.CreateDirectory(aleloHome);

            #region Application commands

            Command Profile()
            {
                var profileCommand = new Command("profile",
                    "Select default, create, delete and list user profiles for Meu Alelo")
                {
                    new Option<bool>(new[] {"--list", "-l"})
                        {Description = $"List available profiles under current PULGA_HOME ({aleloHome})"},
                    new Option<string>(new[] {"--create", "-c"})
                    {
                        Description = $"Create new profiles under current PULGA_HOME ({aleloHome})",
                        Argument = new Argument<string>
                            {Arity = ArgumentArity.ExactlyOne, Name = "name", Description = "Name to new profile"}
                    },
                    new Option<string>(new[] {"--delete", "-d"})
                    {
                        Description = $"Delete a profiles under current PULGA_HOME ({aleloHome})",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ExactlyOne, Name = "delete",
                            Description = "Name of the profile to delete"
                        }
                    },
                    new Option<string>(new[] {"--profile", "-p"})
                    {
                        Description =
                            $"Select a default profile for this session under current PULGA_HOME ({aleloHome})",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ExactlyOne, Name = "profile",
                            Description = "Name of the profile to use"
                        }
                    },
                    new Option<bool>(new[] {"--current-profile"}) {Description = "Profile used by default"}
                };

                profileCommand.Handler =
                    CommandHandler.Create<bool, string, string, string, bool>((list, create, delete, profile,
                        currentProfile) =>
                    {
                        // TODO:
                        // - Add the logic :v
                    });

                return profileCommand;
            }

            Command Card()
            {
                var cardCommand = new Command("card",
                    "Select default and list user cards")
                {
                    new Option<bool>(new[] {"--list", "-l"})
                    {
                        Description =
                            $"List available cards under current profile ({(string.IsNullOrEmpty(aleloDefaultProfile) ? "No profiles created" : aleloDefaultProfile)})"
                    }
                };

                cardCommand.Handler =
                    CommandHandler.Create<bool>(list =>
                    {
                        // TODO:
                        // - Add the logic :v
                    });

                return cardCommand;
            }

            Option Statement()
            {
                var statementOption = new Option(new[] {"-s", "--statement"})
                {
                    Description =
                        $"List last transactions for the default card ({(string.IsNullOrEmpty(aleloDefaultProfile) ? "No profiles created for take card" : aleloDefaultProfile)})"
                };

                return statementOption;
            }

            #endregion

            #region Helpers

            IEnumerable<string> GetProfilesNames(bool withExtension) =>
                withExtension
                    ? Directory.GetFiles(aleloHome)
                        .Where(f => f.EndsWith(".json"))
                    : Directory.GetFiles(aleloHome)
                        .Where(f => f.EndsWith(".json"))
                        .Select(f => f.Replace(".json", string.Empty));

            Func<IEnumerable<string>, IEnumerable<Profile>> getProfiles = profilesWithExtension =>
            {
                var collection = new List<Profile>();

                profilesWithExtension
                    .ToList()
                    .ForEach(async pn =>
                        collection.Add(JsonSerializer.Deserialize<Profile>(await File.ReadAllTextAsync(pn))));

                return collection;
            };

            #endregion

            var commands = new RootCommand
            {
                Profile(),
                Card(),
                Statement()
            };

            commands.Handler = CommandHandler.Create<bool>(statement =>
            {
                // TODO:
                // - Add the logic :v
            });

            commands.Description = "Meu Alelo as a command line interface, but better";
            return await commands.InvokeAsync(args).ConfigureAwait(true);
        }
    }

    internal class Profile
    {

    }

    internal class Session
    {

    }
}