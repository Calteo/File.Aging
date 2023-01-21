using System.Collections;
using System.Text;

namespace File.Aging.Cmd
{
    internal class FormattedCollection : IEnumerable<FormattedColumn>
    {
        public FormattedCollection()
        {
        }

        #region Columns
        private List<FormattedColumn> Columns { get; } = new List<FormattedColumn>();
        public void Add(string caption, FormattedAlignment alignment)
        {
            if (Rows.Count > 0)
                throw new InvalidOperationException("Can not add column after adding rows.");
            
            Columns.Add(new FormattedColumn(caption, alignment));
        }

        IEnumerator<FormattedColumn> IEnumerable<FormattedColumn>.GetEnumerator() => Columns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Columns.GetEnumerator();
        #endregion

        #region Rows
        private List<string[]> Rows { get; } = new List<string[]>();
        public void Add(params string[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length != Columns.Count)
                throw new ArgumentException("Wrong number of values given.", nameof(values));

            var row = new string[Columns.Count];

            for (var i = 0; i < values.Length; i++)
            {
                row[i] = values[i] ?? "";
            }

            Rows.Add(row);
        }
        #endregion

        #region Format()
        public string ToFormatted()
        {
            var widths = Columns.Select(c => c.Caption.Length).ToArray();
            widths = Rows.Aggregate(widths, (w, r) =>
            {
                for (int i = 0; i < r.Length; i++)
                {
                    w[i] = Math.Max(w[i], r[i].Length);
                }
                return w;
            });

            var builder = new StringBuilder();
            for (var i = 0; i < Columns.Count; i++)
            {
                builder.Append(AlignText(Columns[i].Caption, FormattedAlignment.Left, widths[i]));
                builder.Append(' ');
            }
            builder.AppendLine();

            for (var i = 0; i < Columns.Count; i++)
            {
                builder.Append(new string('-', widths[i]));
                builder.Append(' ');
            }
            builder.AppendLine();

            foreach (var row in Rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    builder.Append(AlignText(row[i], Columns[i].Alignment, widths[i]));
                    builder.Append(' ');
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string AlignText(string text, FormattedAlignment alignment, int width)
        {
            text ??= "";

            return alignment switch
            {
                FormattedAlignment.Left => text.PadRight(width),
                FormattedAlignment.Center => text.PadLeft(width / 2).PadRight(width / 2).PadRight(width),
                FormattedAlignment.Right => text.PadLeft(width),
                _ => string.Empty.PadRight(width),
            };
        }
        #endregion

    }

    internal class FormattedColumn
    {
        public FormattedColumn(string caption, FormattedAlignment alignment)
        {
            Caption = caption;
            Alignment = alignment;
        }

        public string Caption { get; }
        public FormattedAlignment Alignment { get; }                
    }

    internal enum FormattedAlignment
    {
        Left,
        Center,
        Right
    }
}
