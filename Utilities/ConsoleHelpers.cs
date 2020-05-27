using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.Utilities
{
    public static class ConsoleHelpers
    {
        public static void PrintAsTable<T>(IReadOnlyList<string> rowHeaders, IReadOnlyList<string> colHeaders, T[,] values, string formatString = "{0}")
        {
            if (rowHeaders == null)
                throw new ArgumentNullException("rowHeaders");

            if (colHeaders == null)
                throw new ArgumentNullException("colHeaders");

            if (values == null)
                throw new ArgumentNullException("values");

            if (rowHeaders.Count != values.GetLength(0))
                throw new InvalidOperationException("rowHeaders length must equal values first length.");

            if (colHeaders.Count != values.GetLength(1))
                throw new InvalidOperationException("colHeaders length must equal values second length.");

            var tableRows = rowHeaders.Count + 1;
            var tableCols = colHeaders.Count + 1;
            var table = new string[tableRows, tableCols];

            for (var ci = 0; ci < colHeaders.Count; ci++)
                table[0, ci + 1] = colHeaders[ci];

            for (var ri = 0; ri < rowHeaders.Count; ri++)
                table[ri + 1, 0] = rowHeaders[ri];

            for (var ci = 0; ci < colHeaders.Count; ci++)
                for (var ri = 0; ri < rowHeaders.Count; ri++)
                    table[ri + 1, ci + 1] = string.Format(formatString, values[ri, ci]);

            var colSizes = Enumerable.Range(0, tableCols).Select(ci =>
                Enumerable.Range(0, tableRows).Max(ri => (table[ri, ci] ?? "").Length)
            )
            .ToList();

            var lineFormat = string.Join(" | ", Enumerable.Range(0, tableCols).Select(ci => string.Format("{{{0},-{1}}}", ci, colSizes[ci])));

            for (var ri = 0; ri < tableRows; ri++)
                Console.WriteLine(lineFormat, (object[])Enumerable.Range(0, tableCols).Select(ci => table[ri, ci]).ToArray());
        }

        public static void PrintAsList<T>(IReadOnlyList<string> rowHeaders, IReadOnlyList<string> colHeaders, T[,] values, string formatString = "{0}")
        {
            if (rowHeaders == null)
                throw new ArgumentNullException("rowHeaders");

            if (colHeaders == null)
                throw new ArgumentNullException("colHeaders");

            if (values == null)
                throw new ArgumentNullException("values");

            if (rowHeaders.Count != values.GetLength(0))
                throw new InvalidOperationException("rowHeaders length must equal values first length.");

            if (colHeaders.Count != values.GetLength(1))
                throw new InvalidOperationException("colHeaders length must equal values second length.");

            for (var ri = 0; ri < rowHeaders.Count; ri++)
            {
                Console.WriteLine("{0}: {1}", rowHeaders[ri], string.Join(
                    " ",
                    Enumerable.Range(0, colHeaders.Count).Select(ci =>
                        string.Format("{0}: {1}", colHeaders[ci], string.Format(formatString, values[ri, ci]))
                    )
                ));
            }
        }
    }
}
