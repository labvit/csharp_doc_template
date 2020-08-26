//Rextester.Program.Main is the entry point for your code. Don't change it.
//Microsoft (R) Visual C# Compiler version 2.9.0.63208 (958f2354)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rextester
{
    public class l{
     public static Action<string>  g=>Console.WriteLine;
    }
    public class Row : List<string>
    {
        
    }
	public enum TagType
	{
		rowspan,
		colspan
	}
	public class Attributes
	{
		//public Tag Parent;
		public string Name;
		public string Value;
	}

    public class TagAttribute
    {
    }        
    
	public class Tag
	{
        public int RowIndex;
        public int ColumnIndex;
		public TagType Type;
        public bool IsEnd => Name.Contains("</");
        public bool IsClosed => Name.Contains("/>");
		public string Name;
		public IEnumerable<TagAttribute> Attributes;
        public int start;
	}

    public class DataRow
    {
        Dictionary<string, string> _Cells = new Dictionary<string,string>();
        public string this[string key]        
        {
            get
            {
                return _Cells[key];
            }
            set
            {
                if(_Cells.ContainsKey(key)) _Cells.Add(key, value);
                else _Cells[key] = value;
            }
        }
    }
    
    public class DataTable
    {
        public List<DataRow>  Rows;
    }
    
    public class DataSet
    {
        public Dictionary<string,DataTable> Tables;
    }
    
	public class TagFactory
    {
        ///
        ///  parse content between < ... >
        ///
        public static Tag Parse(string txt, int rowI, int colI, int insertI)
        {
            
            return new Tag{start = insertI, Name = txt, RowIndex = rowI, ColumnIndex = colI };
        }
    }
    
    public class Range
    {
        public Tag Start;
        public Tag End;
        
        public Range(Tag tag)
        {
            Start = End = tag;
        }
        public Range(Tag start, Tag end)
        {
            Start = start;
            End = end;
        }
        public bool IsUnitTag => Start == End;
    }
    
    public class TaggedTable
    {
        List<Row > _Table = new List<Row>(); 
        List<Range> _Ranges = new List<Range>();
        Dictionary<Range,Range> _Relation = new Dictionary<Range,Range>();
        
        public List<Tag> Tags;
        public List<Range> Ranges => _Ranges;
        public Dictionary<Range,Range> Relation => _Relation;
        public TaggedTable(){ Tags = new List<Tag>();}
        
        public Row NewRow()
        {
            _Table.Add(new Row());
            return _Table.Last();
        }
        
        public void AddRow(Row row)
        {
            _Table.Add(row);
        }
        public List<Row > Data => _Table;

        public void PostProcess()
        {
            var orderedList = Tags.OrderBy(x=>x.RowIndex).ThenBy(x=>x.ColumnIndex);
            List<Range> ranges = new List<Range>();
            Stack<Range> stack = new Stack<Range>();
            Dictionary<Range,Range> relation = new Dictionary<Range,Range>();
            foreach(var cell in orderedList)
            {
                l.g(""+ cell.Name + " " + cell.IsClosed + " " + cell.IsEnd);
                if(cell.IsClosed)
                    ranges.Add(new Range(cell));
                else
                {
                    Range rng = null;
                    if(cell.IsEnd)
                    {
                        rng = stack.Pop();
                        rng.End = cell;
                        ranges.Add(rng);
                    }
                    else
                    {
                        rng = new Range(cell,null);
                        if(stack.Any())
                            relation.Add(stack.Peek(),rng);
                        stack.Push(rng);
                    }
                    
                }
            }
            l.g("stack size "+stack.Count);
            _Ranges = ranges;
            _Relation = relation;
            l.g("rel = " + _Relation.Count);
        }
    }
    
    public class TableProcess
    {
        TaggedTable _Table;
        public TableProcess( TaggedTable table )
        {
            _Table = table;
        }
        
        public List<Row> Process(DataSet set)
        {
            //_Table.Data;
            foreach(var range in _Table.Ranges)
            {
                if(set.Tables.ContainsKey(range.Start.Name))
                {
                    
                }
            }
            return null;
        }
    }
    
    public class TextProcess
    {
        Func<IEnumerable<string>> _LineSplit;
        Func<string, IEnumerable<string>> _CellSplit;
        public TextProcess(Func< IEnumerable<string>> line, Func<string, IEnumerable<string>> cell)
        {
                _LineSplit = line;
                _CellSplit = cell;
        }
        public TaggedTable Process()
        {
            TaggedTable table = new TaggedTable();
            int rowIndex = 0;
            foreach(var line in _LineSplit())
            {
                rowIndex ++;
                var cells = line.Split(';');
                Row row = table.NewRow();
                for(int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];

                    List<Tag> tags = new List<Tag>();
                    List<int> ends = new List<int>();
                    int startTag = -1;
                    for(int _ = 0; _ < 100; _++)
                    {
                        startTag = cell.IndexOf('<', startTag + 1);
                        if(startTag == -1)
                            break;
                        var endTag = cell.IndexOf('>', startTag);
                        if(endTag == -1)
                            break;
                        tags.Add(TagFactory.Parse(cell.Substring(startTag, endTag - startTag+1), rowIndex, i+1, startTag));
                        ends.Add(endTag);
                        
                    }
                    
                    List<string> builder = new List<String>();
                    
                    if(tags.Any())
                    {
                        int oldJ = 0;
                        builder.Add(cell.Substring(0,tags[0].start));
                        for(int j = 1 ; j < tags.Count; j++)
                        {
                            builder.Add(cell.Substring(ends[oldJ] +1, tags[j].start - ends[oldJ]-1));
                            oldJ = j;
                        }
                        
                        if(ends.Last() < cell.Length)
                            builder.Add(cell.Substring(ends[oldJ]+1))   ;
                        
                        row.Add(string.Join("", builder));
                        
                        table.Tags.AddRange(tags);
                    }
                    else
                        row.Add(cell);
                }
            }            
            return table;
        }
        
    }

class Program
{
    public static Action<string> log => Console.WriteLine;
	public static void Main(String[] args)
	{
		string input = "<s>cell1<b/>b;<d> cel</d> l2 ;</s>cell3\ns;s;";
        var table = (new TextProcess(()=> input.Split('\n'), s=>s.Split(';') )).Process();
        table.PostProcess();
        log("" + table.Ranges.Count);
        Stack<Range> stack = new Stack<Range>();
        
        
        foreach(var rng in table.Ranges)
        {
            stack.Push(rng);
            if(!rng.IsUnitTag)
            {
                l.g("" + rng.Start.Name + " " + rng.End.Name);
                if(table.Relation.ContainsKey(rng))
                {
                    table.Relation[rng];
                }
                //l.g("" + rng.Start.);
            }
            else
                l.g(rng.Start.Name);
        }
        foreach(var row in table.Data)
            {
                foreach(var cell in row)
                {
                  Console.Write(cell + ";" );
                }
              Console.WriteLine();
            }
        
     }
}
}
