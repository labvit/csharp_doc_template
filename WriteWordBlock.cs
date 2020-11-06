 class Block
        {
            public const bool RowAllign = true;

            Word.Table _Table;
            private Word.Table myTable;
            private int startRowCount;

            public Block(Word.Table myTable, int startRowCount)
            {
                this.myTable = myTable;
                this.startRowCount = startRowCount;
            }

            List<Tuple<int, int, string[], bool>> _Cells = new List<Tuple<int, int, string[], bool>>();

            internal Block cell(int row, int column, string txt, bool isRowAllign = false)
            {
                if(row == -1)
                    for(int i = _Cells.Count-1; i <= 0; i--)
                    {
                        if (_Cells[i].Item2 == column && _Cells[i].Item1 > -1)
                            _Cells[i] = Tuple.Create(_Cells[i].Item1, _Cells[i].Item2, _Cells[i].Item3.Concat(ValuesCells(_Table.Columns[column].Width, txt)).ToArray(), _Cells[i].Item4); 
                    }
                else
                    _Cells.Add(Tuple.Create(row, column, ValuesCells(_Table.Columns[column].Width, txt).ToArray(), isRowAllign));
                return this;
            }

            internal void endBlock(out int rowsOffset)
            {
                foreach (var row in _Cells.OrderBy(x=>x.Item1).GroupBy(x=>x.Item1))
                {
                    Dictionary<int, int> rowCount = new Dictionary<int, int>();
                    foreach (var cell in row.OrderBy(x=>x.Item2))
                    {

                    }
                }
            }
            #region private
            private static int CellWrite(int row, int column, Microsoft.Office.Interop.Word.Table myTable, string txt)
            {
                myTable.Cell(row, column).Range.Text = txt;
                return 1;
            }
            private static int CellsWrite(int row, int column, Microsoft.Office.Interop.Word.Table myTable, string txt)
            {
                var mYcellValue = ValuesCells(myTable.Columns[column].Width, txt);
                _CellsWrite(new int[] { row, column }, myTable, mYcellValue);
                return mYcellValue.Count;
            }
            private static void _CellsWrite(int[] posRC, Microsoft.Office.Interop.Word.Table myTable, IEnumerable<string> sb)
            {
                int i = 0;
                foreach (string str in sb)
                {
                    while (!IsExistRow(myTable, posRC[0] + i))
                        myTable.Rows.Add();
                    myTable.Cell(posRC[0] + i, posRC[1]).Range.Text = str;
                    i += 1;
                }
            }
            private static List<string> ValuesCells(float widthCell, string value)
            {
                List<string> result = new List<string>();
                int oldPos = 0;
                for (int newlinePos = value.IndexOf(Environment.NewLine); newlinePos != -1; newlinePos = value.IndexOf(Environment.NewLine, newlinePos + 1))
                {
                    result.AddRange(_ValuesCells(widthCell, value.Substring(oldPos, newlinePos - oldPos)));
                    oldPos = newlinePos;
                }
                result.AddRange(_ValuesCells(widthCell, value.Substring(oldPos)));
                return result;
            }
            private static List<string> _ValuesCells(float widthCell, string value)
            {
                List<string> v = new List<string>();
                int interval = (int)Math.Floor(widthCell / (10.4 / 2));
                //int interval = (int)Math.Floor(widthCell / (12 ));

                string[] asdasdad = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string onestr = "";
                foreach (string str in asdasdad)
                {

                    if (onestr.Length == 0)
                        onestr = str;
                    else if ((onestr + ' ' + str).Length <= interval)
                        onestr += ' ' + str;
                    else
                    {
                        v.Add(onestr);
                        onestr = str;
                    }
                }

                if (onestr.Length != 0)
                    v.Add(onestr);
                /*   while(interval<value.Length)
                   {
                       v.Add(value.Substring(0, interval));
                       value = value.Substring(interval);
                   }
                   if(value.Length>0)
                       v.Add(value);*/
                return v;
            }
            private static bool IsExistRow(Microsoft.Office.Interop.Word.Table table, int row)
            {
                if (table.Rows.Count >= row)
                    return true;
                else
                    return false;
            }

            #endregion
        }
