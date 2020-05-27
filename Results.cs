using SearchFight.SearchRunners;
using SearchFight.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight
{
    public class Results
    {
        private readonly Lazy<string[,]> _CountsTable;
        private readonly Lazy<IReadOnlyList<KeyValuePair<string, string>>> _Winners;
        private readonly Lazy<string> _Winner;
        private readonly Lazy<string> _NormalizedWinner;

        public readonly IReadOnlyList<string> Languages;
        public readonly IReadOnlyList<string> Runners;
        public readonly long[,] Counts;


        public string[,] CountsTable
        {
            get { return _CountsTable.Value; }
        }

        public IReadOnlyList<KeyValuePair<string, string>> Winners
        {
            get { return _Winners.Value; }
        }

        public string Winner
        {
            get { return _Winner.Value; }
        }

        /// <summary>
        /// Gets the winner considering the total number of results in each search runner.
        /// </summary>
        public string NormalizedWinner
        {
            get { return _NormalizedWinner.Value; }
        }

        private Results(IReadOnlyList<string> languages, IReadOnlyList<string> runners, long[,] counts)
        {
            if (languages == null)
                throw new ArgumentNullException("languages");

            if (runners == null)
                throw new ArgumentNullException("runners");

            if (counts == null)
                throw new ArgumentNullException("results");

            if (languages.Count != counts.GetLength(0))
                throw new InvalidOperationException("Counts first length must equal languages length.");

            if (runners.Count != counts.GetLength(1))
                throw new InvalidOperationException("Counts second length must equal ruunners length.");

            Languages = languages;
            Runners = runners;
            Counts = counts;

            _CountsTable = new Lazy<string[,]>(GetCountsTable);
            _Winners = new Lazy<IReadOnlyList<KeyValuePair<string, string>>>(GetWinners);
            _Winner = new Lazy<string>(GetWinner);
            _NormalizedWinner = new Lazy<string>(GetNormalizedWinner);
        }

        private string[,] GetCountsTable()
        {
            var table = new string[Languages.Count + 1, Runners.Count + 1];

            for (var ci = 0; ci < Runners.Count; ci++)
                table[0, ci + 1] = Runners[ci];

            for (var ri = 0; ri < Languages.Count; ri++)
                table[ri + 1, 0] = Languages[ri];

            for (var ci = 0; ci < Runners.Count; ci++)
                for (var ri = 0; ri < Languages.Count; ri++)
                    table[ri + 1, ci + 1] = string.Format("{0:n0}", Counts[ri, ci]);

            return table;
        }

        private IReadOnlyList<KeyValuePair<string, string>> GetWinners()
        {
            var result = new KeyValuePair<string, string>[Runners.Count];

            for (var ri = 0; ri < Runners.Count; ri++)
            {
                var winnerIndex = Languages.Indexes().Select(li => Counts[li, ri]).MaxIndex();
                var winner = Languages[winnerIndex];
                result[ri] = new KeyValuePair<string, string>(Runners[ri], Languages[winnerIndex]);
            }

            return result;
        }

        private string GetWinner()
        {
            var winnerIndex = Languages.Indexes().Select(li =>
                Runners.Indexes().Sum(ri => Counts[li, ri])
            )
            .MaxIndex();

            return Languages[winnerIndex];
        }

        private string GetNormalizedWinner()
        {
            var sums = Runners.Indexes().Select(ri =>
                Math.Max((double)Languages.Indexes().Select(li => Counts[li, ri]).Sum(), 1.0)
            )
            .ToList();

            var winnerIndex = Languages.Indexes().Select(li =>
                Runners.Indexes().Sum(ri => (double)Counts[li, ri] / sums[ri])
            )
            .MaxIndex();

            return Languages[winnerIndex];
        }

        public static async Task<Results> Collect(IReadOnlyList<string> languages, IReadOnlyList<ISearchRunner> runners, IProgressReporter progressReporter = null)
        {
            if (languages == null)
                throw new ArgumentNullException("languages");

            if (runners == null)
                throw new ArgumentNullException("runners");

            var results = new long[languages.Count, runners.Count];
            var errors = new List<Exception>();

            if (progressReporter != null)
            {
                progressReporter.Initialize(languages.Count * runners.Count);
            }

            List<Task> tasks = new List<Task>();

            for (var li = 0; li < languages.Count; li++)
            {
                for (var ri = 0; ri < runners.Count; ri++)
                {
                    tasks.Add(StartTask(languages, runners, progressReporter, results, li, ri));
                }
            }

            await Task.WhenAll(tasks.ToArray());
            return new Results(languages, runners.Select(r => r.Name).ToList(), results);
        }

        private static async Task StartTask(IReadOnlyList<string> languages, IReadOnlyList<ISearchRunner> runners, IProgressReporter progressReporter, long[,] results, int li, int ri)
        {
            var arg = languages[li];
            var runner = runners[ri];

            try
            {
                var result = await runner.Run(arg);
                results[li, ri] = result;
            }
            catch (ConfigurationException ex)
            {
                throw new SearchException(arg, runner.Name, string.Format(ex.Message, arg, runner.Name), ex);
            }
            catch (WebRequestException ex)
            {
                throw new SearchException(arg, runner.Name, string.Format(ex.Message, arg, runner.Name), ex);
            }
            catch (ParsingException ex)
            {
                throw new SearchException(arg, runner.Name, string.Format(ex.Message, arg, runner.Name), ex);
            }

            if (progressReporter != null)
                progressReporter.Advance();
        }
    }
}
