using ImageProcessor;
using ImageProcessor.Imaging.Formats;

/**
 * Quick and dirty program to convert common annoyances when saving files from the web such as:
 * - Renames *.jpg_small, *.jpg_medium, *.jpg_large, *.jfif etc... saved from Twitter to *.jpg
 * - converts *.webp to *.png.
 * 
 * To run, open a console and cd to the directory you want to fix images on, then call this program.
 * It should perform the above conversions on all files it detects, in the working directory and all
 * sub directories.
 */

try
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var files = new List<string>(Directory.EnumerateFiles(currentDirectory, "*.*", SearchOption.AllDirectories));
    foreach(var file in files)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(file);
        var directory = Path.GetDirectoryName(file);
        var ext = Path.GetExtension(file);
        
        if(!string.IsNullOrEmpty(ext))
        {
            // Convert *.jpg_whatever, *.jfif crap from Twitter to normal *.jpg
            if(ext.StartsWith(".jpg_") || ext.StartsWith(".jpg-") || ext.Equals(".jfif"))
            {
                try
                {
                    var renamedFile = Path.ChangeExtension(file, "jpg");
                    if (!File.Exists(renamedFile))
                    {
                        File.Move(file, renamedFile);
                        Console.WriteLine($"JPG: Changed the extension of '{file}' to .jpg.");
                    }
                    else
                    {
                        Console.WriteLine($"JPG: Cannot change extension of '{file}' to .jpg. A file already exists with that name.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Write($"Error changing extension on '{file}':");
                    Console.WriteLine(ex.ToString());
                }
            }

            // Convert WEBP to PNG (preserves old *.webp files)
            else if(ext.Equals(".webp"))
            {
                try
                {
                    var newPngName = $"{Path.Combine(directory!, nameWithoutExt)}.png";
                    if(!File.Exists(newPngName))
                    {
                        using var newPngStream = new FileStream(newPngName, FileMode.Create);
                        using var imageFactory = new ImageFactory(preserveExifData: true);
                        imageFactory.Load(file)
                                    .Format(new PngFormat())
                                    .Quality(100)
                                    .Save(newPngStream);

                        Console.WriteLine($"WEBP: Converted '{file}' to {newPngName}.");
                    }
                    else
                    {
                        Console.WriteLine($"WEBP: Can't convert '{file}' to {newPngName}. A file already exists with that name.");
                    }
                }
                catch(Exception ex)
                {
                    Console.Write($"Error converting '{file}':");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
