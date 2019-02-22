using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;

namespace MakeMp3
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("Just give me a directory path. I'll do the rest.");
                return;
            }

            var directoryPath = args[0];

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("That's either not a directory or does not exist. Come on!");
                return;
            }

            var extensions = new[] {".wav", ".wma", ".aac"};

            Console.WriteLine("Ok. Here we go. Hang on to something.\n");

            foreach (var filePath in Directory.EnumerateFiles(directoryPath))
            {
                var extension = Path.GetExtension(filePath);

                if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"I'm not sure what to do with \"{Path.GetFileName(filePath)}\". I don't know how to handle that type of file. Sorry. ;)");
                    continue;
                }

                if (string.Equals(extension, ".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Hey, hey! \"{Path.GetFileName(filePath)}\" is already converted. Job done! ^_^");
                    continue;
                }

                var targetFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".mp3");

                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine($"\"{Path.GetFileName(targetFilePath)}\" is already here. Maybe some other converter did that. Possibly?");
                    continue;
                }

                Console.Write($"Converting \"{Path.GetFileName(filePath)}\" to \"{Path.GetFileName(targetFilePath)}\". Abra cadabra ... ");

                ConvertToMP3(filePath, targetFilePath);

                var filePathTag = TagLib.File.Create(filePath);
                var targetFilePathTag = TagLib.File.Create(targetFilePath);

                filePathTag.Tag.CopyTo(targetFilePathTag.Tag, true);

                targetFilePathTag.Save();

                Console.WriteLine($"Ta dah!");
            }
        }

        static void ConvertToMP3(string sourceFilename, string targetFilename)
        {
            using (var reader = new NAudio.Wave.AudioFileReader(sourceFilename))
            using (var writer = new NAudio.Lame.LameMP3FileWriter(targetFilename, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD))
            {
                reader.CopyTo(writer);
            }
        }
    }
}
