using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Text.Json.Serialization;

using static System.Console;
using CommandLine.Text;
using CommandLine;

namespace Card.Cli
{
    internal class Options
    {
        [Option("alelo", Group ="provider", Required = false, HelpText ="Provider Alelo card.")]
        public bool Alelo { get; set; }
        
        [Option("nubank", Group = "provider", Required = false, HelpText = "Provider nubank card.")]
        public bool  Nubank { get; set; }

        [Option("vr", Group = "provider", Required = false, HelpText = "Provider vr card.")]
        public bool Vr { get; set; }       
    }

    internal static class App
    {
        static void Main(string[] args)
            => Parser.Default.ParseArguments<Options>(args).WithParsed( o => AppArgument(o));
        
        static void AppArgument(Options options)
        {
           
        }
    }
}