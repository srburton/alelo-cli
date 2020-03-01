using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Text.Json.Serialization;

using static System.Console;

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

            var globalVerbose = false;

            if (!Directory.Exists(aleloHome))
                Directory.CreateDirectory(aleloHome);

            #region Profile management

            async Task CreateProfile(string profileName)
            {
                if (string.IsNullOrEmpty(profileName.Trim()))
                {
                    WriteLine("[!] Invalid profile name!");
                    Environment.Exit(1);
                }

                profileName = profileName.Trim();

                if (GetProfilesNames(false).Contains(profileName))
                {
                    WriteLine("[!] Profile name already in use!");
                    Environment.Exit(1);
                }

                try
                {
                    await using var fs = File.Create(Path.Combine(aleloHome, profileName + ".json"));
                    await JsonSerializer.SerializeAsync(fs, new Profile
                    {
                        Name = profileName
                    });
                }
                catch (IOException err)
                {
                    WriteLine("[!] Error when creating the profile file!");
                    WriteLine($" > {err.Message}");

                    Environment.Exit(1);
                }
                catch (Exception err)
                {
                    WriteLine("[!] Unknown error happened!");
                    WriteLine($" > {err.Message}");

                    Environment.Exit(1);
                }

                WriteLine($"[+] Profile {profileName} created under current ALELO_HOME ({aleloHome})");
            }

            #endregion

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
                    CommandHandler.Create<bool, string, string, string, bool>(async (list, create, delete, profile,
                        currentProfile) =>
                    {
                        if (list)
                        {
                            WriteLine("[+] Available profiles:");
                            GetProfilesNames(false)
                                .Select(p =>
                                {
                                    if (aleloDefaultProfile == p)
                                        p += " (Current default profile)";

                                    return p;
                                })
                                .ToList()
                                .ForEach(p => WriteLine($" - {p}"));
                            WriteLine();
                        }
                           
                        if (!string.IsNullOrEmpty(delete))
                        {
                            delete = delete.Trim();

                            if (!GetProfilesNames(false).Contains(delete))
                            {
                                WriteLine("[!] Profile not found!");
                                Environment.Exit(1);
                            }

                            File.Delete(Path.Combine(aleloHome, delete + ".json"));
                            WriteLine($"[+] Profile {delete} removed");
                        }

                        if (!string.IsNullOrEmpty(create))
                            await CreateProfile(create);

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

            static Option Verbose()
            {
                var statementOption = new Option(new[] {"-v", "--verbose"})
                {
                    Description = "Increase the application verbose"
                };

                return statementOption;
            }

            #endregion

            #region Helpers

            IEnumerable<string> GetProfilesNames(bool withExtension) =>
                withExtension
                    ? Directory.GetFiles(aleloHome)
                        .Where(f => f.EndsWith(".json"))
                        .Select(Path.GetFileName)
                    : Directory.GetFiles(aleloHome)
                        .Where(f => f.EndsWith(".json"))
                        .Select(f => f.Replace(".json", string.Empty))
                        .Select(Path.GetFileName);

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
                Statement(),
                Verbose()
            };

            commands.Handler = CommandHandler.Create<bool, bool>((statement, verbose) =>
            {
                if (verbose)
                    globalVerbose = true;

                if (!GetProfilesNames(false).Any())
                {
                    WriteLine("[!] No profiles found, create one first!");
                    WriteLine(" > Try --help");

                    Environment.Exit(1);
                }

                if (GetProfilesNames(false).Count() > 1 && string.IsNullOrEmpty(aleloDefaultProfile))
                {
                    WriteLine("[!] More than one profile found!");
                    WriteLine(" > Try --help");

                    Environment.Exit(1);
                }

                if (globalVerbose)
                    WriteLine($"[VERBOSE] Selected profile {aleloDefaultProfile}");
            });

            commands.Description = "Meu Alelo as a command line interface, but better";
            return await commands.InvokeAsync(args).ConfigureAwait(true);
        }
    }

    internal class Profile
    {
        public string Name { get; set; }

        public Session Session { get; set; }
    }

    internal class Session
    {
        [JsonPropertyName("token")] public string Token { get; set; }

        [JsonPropertyName("email")] public string EmailAddress { get; set; }

        [JsonPropertyName("firstName")] public string FirstName { get; set; }

        [JsonPropertyName("lastName")] public string LastName { get; set; }

        [JsonIgnore] public string FullName => FirstName + LastName;

        [JsonPropertyName("gender")] public string Gender { get; set; }

        [JsonPropertyName("maskedEmail")] public string MaskedEmailAddress { get; set; }

        [JsonPropertyName("cpf")] public string Document { get; set; }

        [JsonPropertyName("birthDate")] public string BirthDate { get; set; }

        [JsonPropertyName("ddd")] public string Ddd { get; set; }

        [JsonPropertyName("phone")] public string Phone { get; set; }

        [JsonIgnore] public string FullPhone => Ddd + Phone;

        [JsonPropertyName("userId")] public string UserId { get; set; }
    }
}