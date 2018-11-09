using log4net;
using MovieDataExtractor.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieDataExtractor.OscarOrg
{
    public class OscarOrgFileSanitizer : BaseFileSanitizer
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The config for the extraction of the data
        /// </summary>
        OscarOrgExtractorSettings config;

        /// <summary>
        /// The file which contains all the awards details of the oscar awards
        /// </summary>
        TextReader awardReader;

        /// <summary>
        /// The file which contains all the awards details of the oscar awards
        /// </summary>
        TextWriter awardWriterNormalize;

        /// <summary>
        /// The file which contains all the movies of the oscar awards
        /// </summary>
        TextWriter moviesWriter;

        /// <summary>
        /// The file which contains all the persons of the oscar awards
        /// </summary>
        TextWriter personsWriter;

        /// <summary>
        /// The file which contains all the persons of the oscar awards
        /// </summary>
        TextWriter awardTypeWriter;

        /// <summary>
        /// The file which contains all the songs of the oscar awards
        /// </summary>
        TextWriter songsWriter;

        /// <summary>
        /// The file which contains all the jobs
        /// </summary>
        TextWriter jobsWriter;

        /// <summary>
        /// The file which contains all the jobs and person combination
        /// </summary>
        TextWriter personJobsWriter;

        /// <summary>
        /// Cache
        /// </summary>
        OscarOrgAwardNormalizeCache cache;

        /// <summary>
        /// Constructor
        /// </summary>
        public OscarOrgFileSanitizer()
        {
            config = OscarOrgExtractorSettings.Default;
            cache = new OscarOrgAwardNormalizeCache();
        }

        /// <summary>
        /// Execute the file normalize
        /// </summary>
        public void Run()
        {
            using (awardReader = new StreamReader(config.FileNameAwards))
            using (awardWriterNormalize = new StreamWriter(config.FileNameAwardsNormalize))
            using (moviesWriter = new StreamWriter(config.FileNameMovies))
            using (personsWriter = new StreamWriter(config.FileNamesPerson))
            using (awardTypeWriter = new StreamWriter(config.FileNameAwardType))
            using (songsWriter = new StreamWriter(config.FileNameMusicSong))
            using (jobsWriter = new StreamWriter(config.FileNameJob))
            using (personJobsWriter = new StreamWriter(config.FileNamePersonJob))
            {
                // Write header
                awardWriterNormalize.WriteLine(config.ColumnNameAwardsNormalize);
                moviesWriter.WriteLine(config.ColumnNameMovies);
                personsWriter.WriteLine(config.ColumnNamePerson);
                awardTypeWriter.WriteLine(config.ColumnNameAwardType);
                songsWriter.WriteLine(config.ColumnNameMusicSong);
                jobsWriter.WriteLine(config.ColumnNameJob);
                personJobsWriter.WriteLine(config.ColumnNamePersonJob);

                cache.Initialize();
                cache.InitializeYear();

                var awardLine = "";
                while ((awardLine = awardReader.ReadLine()) != null)
                {
                    if (string.Compare(awardLine, config.ColumnNameAwards, true) == 0) continue;
                    var csvColumns = SplitCSV(awardLine).ToList();
                    var index = Convert.ToInt32(csvColumns[0]);

                    if (cache.YearCache.YearIndex != index)
                    {
                        logger.InfoFormat($"Processing for year {cache.YearCache.YearIndex}");

                        WriteToFiles();
                        cache.LastMovieId += cache.YearCache.Movies.Count;
                        cache.LastSongId += cache.YearCache.Songs.Count;
                        cache.InitializeYear();
                        cache.YearCache.YearIndex = index;
                    }

                    NormalizeLine(csvColumns);
                }

                WriteFinallyAll();
            }
        }

        /// <summary>
        /// Normalize a award line
        /// </summary>
        private void NormalizeLine(List<string> csvColumns)
        {
            cache.YearCache._AwardTypeId = cache.SaveAndGetAwardTypeId(csvColumns[1]);
            cache.YearCache._IsWinner = Convert.ToBoolean(csvColumns[2]);

            if (cache.ContainsOr(csvColumns[1], "ACTOR", "ACTRESS"))
                ParseKeyNameAsPerson(csvColumns);
            else
                ParseForSpecificRegex(csvColumns[4]);
            //else if(awardType.ToUpper().Contains("SONG") && awardType.ToUpper().Contains("MUSIC"))
            //    ParseKeyNameAsSong(awardTypeId, csvColumns);
            //else
            //    ParseKeyNameAsMovie(awardTypeId, csvColumns);
        }

        private string ParseForSpecificRegex(string keyValue)
        {
            if(keyValue.Contains(";"))
            {
                var splitBySemicolon = keyValue.Split(';').Select(i => i.Trim());
                splitBySemicolon.Select<string, string>(ParseForSpecificRegex);
            }
            else if(keyValue.Contains(" by "))
            {
                var splitByby = keyValue.Split(new string[] { "by" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim());
                splitByby.Select<string, string>(ParseForSpecificRegex);
            }
            else if (keyValue.Contains(" and "))
            {
                var splitByby = keyValue.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim());
                splitByby.Select<string, string>(ParseForSpecificRegex);
            }

            var regexPatterns = new List<string>()
            {
                @"(.*)\s+in\s+\|(.*)\|\s+Music\s+and\s+Lyrics\s+by\s+(.*)",
                @"(.*)\s+in\s+\|(.*)\|\s+Music\s+by\s+(.*)[;,]\s+Lyrics\s+by\s+(.*)",
                @"Art\s+Direction\s*:\s*(.*)\s*;\s*Set\s+Decoration\s*:\s*(.*)",
                @"(.*?)(?: and (.*?))?, Producers?"
            };
        }

        private void ParseKeyNameAsPerson(List<string> csvColumns)
        {
            var personId = cache.SaveAndGetPersonId(csvColumns[3]);
            var movieId = cache.YearCache.SaveAndGetMovieId(csvColumns[4]);

            cache.YearCache.SaveRow(movieId, personId);
        }

        private void ParseKeyNameAsSong(int awardTypeId, List<string> csvColumns)
        {
            var movieName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(csvColumns[3].Trim('"', ' ', '–').ToLower());
            var movieId = cache.YearCache.SaveAndGetMovieId(movieName);

            Regex regex = new Regex(@"(.*)\s+in\s+\|(.*)\|\s+Music by\s+(.*)[;,]\s+Lyrics by\s+(.*)");

            var keyValue = csvColumns[4].Trim('"', ' ', '–');
            var match = regex.Match(keyValue);

            if(match.Success)
            {
                var songName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    match.Groups[1].Value.ToLower());
                
                var movieName2 = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    match.Groups[2].Value.ToLower());
                if (movieName2 != movieName)
                    throw new Exception($"{movieName} do not match {movieName2}");

                cache.YearCache.SaveSong(songName, movieId, awardTypeId);

                var musicBy = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    match.Groups[3].Value.ToLower());
                var musicJobId = cache.SaveAndGetJobTypeId("MusicBy");
                var musicianId = cache.SaveAndGetPersonId(musicBy);

                cache.SavePersonJob(musicianId, musicJobId);
                cache.YearCache.SaveRow(awardTypeId, Convert.ToBoolean(csvColumns[2]), movieId, musicianId);

                var lyricsBy = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    match.Groups[4].Value.ToLower());
                var lyricistJobId = cache.SaveAndGetJobTypeId("LyricsBy");
                var lyricistId = cache.SaveAndGetPersonId(lyricsBy);

                cache.SavePersonJob(lyricistId, lyricistJobId);
                cache.YearCache.SaveRow(awardTypeId, Convert.ToBoolean(csvColumns[2]), movieId, lyricistId);
            }
            else
            {
                match = Regex.Match(keyValue, @"(.*)\s+in\s+\|(.*)\|\s+Music and Lyrics by\s+(.*)");

                if(match.Success)
                {
                    var songName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    match.Groups[1].Value.ToLower());

                    var movieName2 = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        match.Groups[2].Value.ToLower());
                    if (movieName2 != movieName)
                        throw new Exception($"{movieName} do not match {movieName2}");

                    cache.YearCache.SaveSong(songName, movieId, awardTypeId);

                    var musicAndLyricsBy = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        match.Groups[3].Value.ToLower());
                    var musicJobId = cache.SaveAndGetJobTypeId("MusicBy");
                    var musicianId = cache.SaveAndGetPersonId(musicAndLyricsBy);

                    cache.SavePersonJob(musicianId, musicJobId);
                    cache.YearCache.SaveRow(awardTypeId, Convert.ToBoolean(csvColumns[2]), movieId, musicianId);

                    var lyricistJobId = cache.SaveAndGetJobTypeId("LyricsBy");
                    var lyricistId = cache.SaveAndGetPersonId(musicAndLyricsBy);

                    cache.SavePersonJob(lyricistId, lyricistJobId);
                    cache.YearCache.SaveRow(awardTypeId, Convert.ToBoolean(csvColumns[2]), movieId, lyricistId);
                }
                else
                {
                    throw new Exception($"{awardTypeId} has error in Song interpretation for {keyValue}");
                }
            }
        }

        private void ParseKeyNameAsMovie(int awardTypeId, List<string> csvColumns)
        {
            var movieName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(csvColumns[3].Trim('"', ' ', '–').ToLower());
            var movieId = cache.YearCache.SaveAndGetMovieId(movieName);

            var keyValue = csvColumns[4].Trim('"', ' ', '–');
            var keyValueList = keyValue.Split(',').Select(p => p.Trim()).ToList();

            keyValueList = NormalizeForJuniorPrefix(keyValueList);

            // Supposed to be all persons
            for (int i = 0; i < keyValueList.Count; i++)
            {
                var personName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(keyValueList[i].ToLower());
                var personId = cache.SaveAndGetPersonId(personName);
                cache.YearCache.SaveRow(awardTypeId,
                    Convert.ToBoolean(csvColumns[2]), movieId, personId);
            }
        }

        private List<string> NormalizeForJuniorPrefix(List<string> keyValueList)
        {
            if (keyValueList == null || keyValueList.Count <= 0) return keyValueList;

            var normList = new List<string>();
            normList.Add(keyValueList[0]);
            for (int i = 1, j = 0; i < keyValueList.Count; i++)
            {
                if (keyValueList[i].ToLower().Contains("jr."))
                    normList[j] = $"{keyValueList[i]} {normList[j]}";
                else
                {
                    normList.Add(keyValueList[i]);
                    j = normList.Count - 1;
                }
            }

            return normList;
        }

        

        /// <summary>
        /// Write to files
        /// </summary>
        private void WriteToFiles()
        {
            for (int i = 0; i < cache.YearCache.awardRows.Count; i++)
            {
                var row = cache.YearCache.awardRows[i];
                awardWriterNormalize.WriteLine(
                    $"{row.Item1},{row.Item2},{row.Item3},{row.Item4},{row.Item5}");
            }

            awardWriterNormalize.Flush();

            for (int i = 0; i < cache.YearCache.Movies.Count; i++)
            {
                moviesWriter.WriteLine($"{i + cache.LastMovieId},\"{cache.YearCache.Movies[i]}\"");
            }

            moviesWriter.Flush();

            for (int i = 0; i < cache.YearCache.Songs.Count; i++)
            {
                var row = cache.YearCache.Songs[i];
                songsWriter.WriteLine($"{i + cache.LastSongId},{row.Item1},{row.Item2},\"{row.Item3}\"");
            }

            songsWriter.Flush();
        }

        private void WriteFinallyAll()
        {
            for (int i = 0; i < cache.Persons.Count; i++)
            {
                personsWriter.WriteLine($"{i},\"{cache.Persons[i]}\"");
            }

            personsWriter.Flush();

            for (int i = 0; i < cache.AwardTypes.Count; i++)
            {
                awardTypeWriter.WriteLine($"{i},\"{cache.AwardTypes[i]}\"");
            }

            awardTypeWriter.Flush();

            for (int i = 0; i < cache.JobTypes.Count; i++)
            {
                jobsWriter.WriteLine($"{i},\"{cache.JobTypes[i]}\"");
            }

            jobsWriter.Flush();

            for (int i = 0; i < cache.PersonJobs.Count; i++)
            {
                personJobsWriter.WriteLine($"{cache.PersonJobs[i].Item1},\"{cache.PersonJobs[i].Item2}\"");
            }

            personJobsWriter.Flush();
        }
    }
}
