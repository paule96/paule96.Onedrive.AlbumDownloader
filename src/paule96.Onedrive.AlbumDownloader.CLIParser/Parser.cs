using paule96.Onedrive.AlbumDownloader.Models;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.IO;
using System.Linq;
using System.Net.Http;


namespace paule96.Onedrive.AlbumDownloader.CLIParser
{
    public class CliParser
    {
        private static IGraphSessionService graphService = new GraphSessionService("3c5002f6-70c4-425d-bc04-346a93e6376a", new HttpClient());
        public static void Parse(string[] args)
        {
            Console.WriteLine("Hello World!");
            rootCommand().InvokeAsync(args).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static RootCommand rootCommand()
        {
            var command = new RootCommand();
            command.Description = "AlbumLoader is a tool to download you onedrive albums. " + System.Environment.NewLine +
                "" + System.Environment.NewLine +
                "This allows you to list and download you onedrive albums. This helps if you have big albums because this can't be downloaded over onedrive...";
            command.Handler = CommandHandler.Create(() =>
            {
                Console.WriteLine("Please run --help. ");
            });

            // Add all subcommands!

            command.AddCommand(listCommand());
            command.AddCommand(downlodCommand());
            return command;
        }

        private static Command listCommand()
        {
            var command = new Command("list");
            command.Description = "list all your albums";
            command.AddOption(new Option("--search", "provide a text that is in the album title.", new Argument<string>()));
            command.Handler = CommandHandler.Create<string>(async (s) =>
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    // TODO: implement search
                    throw new NotImplementedException("The search option isn't implemented yet.");
                }
                await graphService.Login();
                var albumService = new AlbumService(graphService);
                var result = await albumService.GetAlbums();
                var console = new SystemConsole();
                var tableView = new TableView<Album>() {
                    Items = (await albumService.GetAlbums()).ToList()
                };
                tableView.AddColumn(a => a.name, "Name");
                tableView.AddColumn(a => a.createdDateTime.ToLocalTime().ToLongDateString(), "created");
                tableView.AddColumn(a => a.createdBy.user.displayName, "Author");
                tableView.AddColumn(a => a.lastModifiedDateTime.ToLocalTime().ToLongDateString(), "modifed");
                tableView.AddColumn(A => A.lastModifiedBy.user.displayName, "last Author");
                var consoleRenderer = new ConsoleRenderer(console);
                var screen = new ScreenView(consoleRenderer) { Child = tableView };
                screen.Render();

            });
            return command;
        }

        private static Command downlodCommand()
        {
            var command = new Command("download");
            command.Description = "download all files in an album.";
            command.AddOption(new Option("--id", "the id of the album you will downaload", new Argument<string>()));
            command.AddOption(new Option("--output", "the path of an folder to download the album.", new Argument<string>()));
            command.Handler = CommandHandler.Create<string, string>(async (string id, string output) =>
            {
                if (String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("please provide the parameter id to download an onedrive album.");
                }
                var allFiles = await new AlbumService(graphService).GetAlbumItems(id);
                string path = Path.GetFullPath(output);
                Console.WriteLine("This can make some time. please be patitent.");
                await new FileService(graphService).DownloadFiles(path, allFiles);
            });
            return command;
        }
    }
}
